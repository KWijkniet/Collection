<?php
//Get courses and their corresponding attempts
public function getCourses(Request $request){
    $user = auth()->user();

    //set and the personal variable if needed
    $personal = !isset($request->personal) || $request->personal == false ? false : true;

    $ids = [];
    $data = [];

    //if ids have been given then load data for those courses
    if(isset($request->ids)){
        $ids = $request->ids;
    }
    //if no ids have been given load the data for all courses within that company
    else{
        $ids = Course::where('company_id', $user->company_id)->pluck('id');
    }

    //Load the courses
    foreach ($ids as $id) {
        array_push($data, $this->getCourse($id, $user, $personal));
    }

    //return the loaded courses
    return response()->json($data, 200);
}

private function getCourse($id, $user, $personal){
    //Get the target course
    $course = Course::where('company_id', $user->company_id)->where('id', $id)->first();

    //Get the chapters for that course
    $chapters = CourseChapter::where('course_id', $id)->get();
    foreach ($chapters as &$chapter) {
        //get all "course activities" for the target chapter
        $courseActivities = CourseActivity::where('course_id', $id)->where('chapter_id', $chapter->id)->get();
        $activities = [];

        foreach ($courseActivities as $courseActivity) {
            //get the target activity its versions (where the latest one is the first one in the array)
            $activityVersions = $this->getActivityVersions($courseActivity->activity_id);

            //Loop over all versions and fix their data
            foreach ($activityVersions as &$version) {
                //get all attempts
                $version['attempts'] = $this->getActivityAttempts($course, $courseActivity, $version, $personal);

                //get the content
                $version['content'] = $this->getActivityContent($version);

                //get all competences
                $version['competences'] = $this->getActivityCompetences($version);
            }

            array_push($activities, $activityVersions);
        }
        $chapter['activities'] = $activities;
    }
    $course['chapters'] = $chapters;

    //return the fixed course
    return $course;
}

private function getActivityVersions($id){
    //get the activity (latest of its versions)
    $mainVersion = Activity::where('id', $id)->first();

    //get older versions
    $versions = Activity::where('version_id', $mainVersion->version_id)->where('id', '!=', $mainVersion->id)->get();
    //merge (make sure main is the first item of the array)
    $list = [];
    foreach ($versions as $i => $value) {
        $list[$i + 1] = $value;
    }
    $list[0] = $mainVersion;

    //return versions
    return $versions;
}

private function getActivityCompetences($version){
    //get all competences belonging to this version (filter out doubles)
    $competences = ActivityCompetence::where('activity_id', $version->id)->get();

    //return a fixed array of the competences
    return $this->fixCompetences($competences);
}

private function getActivityContent($version){
    //based on type we can get the required content
    $content = [];

    //module
    if($version->type_id == 2){
        //get all questions for this module
        $content = ModuleQuestion::where('activity_id', $version->id)->get();

        //loop through all questions and fix their data
        foreach ($content as &$question) {
            //get all competences
            $competences = ModuleQuestionCompetence::where("module_question_id", $question->id)->get();
            //fix and then save the competences in the questions data
            $question['competences'] = $this->fixCompetences($competences);
        }
    }

    //return content
    return $content;
}

private function getActivityAttempts($course, $courseActivity, $version, $personal){
    //get all attempts (from everyone if you are allowed to do so) with the corresponding content and competences
    $attempts = [];
    //based on the $personal variable it will either load just yours or all attempts for this version (activity)
    if($personal){
        $attempts = Attempt::where("activity_id", $version->id)->where("course_id", $course->id)->where("course_activity_id", $courseActivity->id)->whereNotNull('completed_at')->where('user_id', auth()->user()->id)->get();
    }else{
        $attempts = Attempt::where("activity_id", $version->id)->where("course_id", $course->id)->where("course_activity_id", $courseActivity->id)->whereNotNull('completed_at')->get();
    }

    //loop through all attempts and fix their data
    foreach ($attempts as &$attempt) {
        //get attempt content if there is any
        $attempt['content'] = $this->getAttemptContent($version, $attempt);
    }

    //return attempts
    return $attempts;
}

private function getAttemptContent($version, $attempt){
    //get content for the attempt
    $content = [];

    //module
    if($version->type_id == 2){
        //get module attempt. this contains a id which is required for the answered questions data.
        $moduleAttempt = ModuleAttempt::where('attempt_id', $attempt->id)->first();
        //get all answers
        $content = ModuleQuestionAttempt::where('module_attempt_id', $moduleAttempt->id)->get();

        foreach ($content as &$answer) {
            //because the questions and answers uuid now match we can get the question belonging to the answer.
            $moduleQuestion = ModuleQuestion::where('uuid', $answer->uuid)->first();
            //get all competences for that question
            $competences = ModuleQuestionCompetence::where("module_question_id", $moduleQuestion->id)->get();
            //fix and store them in the answer
            $answer['competences'] = $this->fixCompetences($competences);
        }
    }

    //return content
    return $content;
}

private function fixCompetences($competences){
    $frameworks = [];
    $masters = [];
    $subs = [];

    //loop over all given competences. each competence contains a framework, master and sub id (if sub id is missing then master is the lowest layer. if master id is missing then framework is the lowest layer).
    foreach ($competences as $competence) {
        //if it contains a framework id then get the framework and add it to the list (if it doesnt already exist)
        if($competence->framework_competence_id != null){
            $framework = FrameworkCompetence::where('id', $competence->framework_competence_id)->first();

            if(!in_array($framework, $frameworks)){
                array_push($frameworks, $framework);
            }
        }
        //if it contains a master id then get the master and add it to the list (if it doesnt already exist)
        if($competence->master_competence_id != null){
            $master = MasterCompetence::where('id', $competence->master_competence_id)->first();

            if(!in_array($master, $masters)){
                array_push($masters, $master);
            }
        }
        //if it contains a sub id then get the sub and add it to the list (if it doesnt already exist)
        if($competence->sub_competence_id != null){
            $sub = SubCompetence::where('id', $competence->sub_competence_id)->first();

            if(!in_array($sub, $subs)){
                array_push($subs, $sub);
            }
        }
    }

    //loop over all masters to fix their data
    foreach ($masters as &$master) {
        $masterSubs = [];
        //loop over each sub to check if it belongs with the target master
        foreach ($subs as &$sub) {
            //add it to the target master if it doesnt already exist
            if($master->id == $sub->master_competence_id && !in_array($sub, $masterSubs)){
                array_push($masterSubs, $sub);
            }
        }
        $master['subs'] = $masterSubs;
    }

    //loop over all frameworks to fix their data
    foreach ($frameworks as &$framework) {
        $frameworkMasters = [];
        //loop over each master to check if it belongs with the target framework
        foreach ($masters as &$master) {
            //add it to the target framework if it doesnt already exist
            if(isset($framework) && $framework->id == $master->framework_competence_id && !in_array($master, $frameworkMasters)){
                array_push($frameworkMasters, $master);
            }
        }
        $framework['masters'] = $frameworkMasters;
    }

    return $frameworks;
}