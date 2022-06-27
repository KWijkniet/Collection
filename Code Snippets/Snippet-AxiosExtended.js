axiosRequest: function (type, url, data, optional = {}){
    //get loader component
    var loader = this.$root.loader;
    
    //create loader item
    var loaderItem = null;
    if(loader){
        loaderItem = loader.add(url);
    }

    //execute a new axios request based on given variables
    return new Promise((res, rej) => {
        axios[type](url, data, optional)
        .then((response) => {
            //remove loader item
            if(loader){
                loader.remove(loaderItem);
            }

            //return the response as a success
            res(response);
        })
        .catch((err) =>{
            //remove loader item
            if(loader){
                loader.remove(loaderItem);
            }

            //return the response as an error
            rej(err);
        })
    });
},
