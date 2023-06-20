/*
    Function: image
    Description: Upload an image.
    name            type        description
    - file:         file:       the image file (not modified)
    - activity_id:  int:        name of target directory (less then 0 will remove the activity subdirectory)
    - version:      int:        name of target version in directory (less then 0 will remove the version subdirectory)
    - name:         string:     name of image
    returns string (path to the file)
*/
public function image($file, $activity_id, $version = 0, $name = "cover"){
    //validate file
    $result = $this->validateImage($file);
    if($result['status'] != 'success'){
        return $result;
    }

    //Upload file to local storage
    $path = $this->uploadToLocal($file);
    $fullPath = Storage::disk('companies')->path('') . $path;

    //If uploaded image is not of type "svg", "gif" or "webp"
    if($result['message']['extension'] != 'svg' && $result['message']['extension'] != 'gif' && $result['message']['extension'] != 'webp'){
        //Compress image
        $tmp = Image::make($file)->heighten(500);

        //overwrite file locally
        $tmp->save($fullPath);
    }

    //Generate path for the cloud
    $fileName = $this->filePath;
    if($version >= 0){
        $fileName .= $version . '/';
    }
    if($activity_id >= 0){
        $fileName .= $activity_id . '/';
    }
    $fileName .= $name . '.' . $result['message']['extension'];

    //upload to cloud
    $url = $this->uploadToStorage($fullPath, $fileName);
    //delete local tmp file
    $this->deleteFromLocal($path);

    return $url;
}