filterChapters(){
    var data = this.$parent.course.chapters;
    var self = this;

    if(this.$parent.search.length > 0){
        data = data.filter(function(item){
            var name = item.name.toLowerCase();
            var search = self.$parent.search.toLowerCase();
            return name.indexOf(search) > -1;
        });
    }

    return data;
},