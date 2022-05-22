axiosRequest: function (type, url, data, optional = {}){
    var loader = this.$root.loader;
    var loaderItem = null;
    if(loader){
        loaderItem = loader.add(url);
    }

    return new Promise((res, rej) => {
        axios[type](url, data, optional)
        .then((response) => {
            if(loader){
                loader.remove(loaderItem);
            }
            res(response);
        })
        .catch((err) =>{
            //check if user session is valid
            if(loader){
                loader.remove(loaderItem);
            }
            rej(err);
        })
    });
},