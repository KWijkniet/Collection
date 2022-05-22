<template>
    <div class="container-fluid">
        <div class="card">
            <div class="card-body">
                <h2>
                    {{$t('profile_picture').charAt(0).toUpperCase() + $t('profile_picture').slice(1)}}
                </h2>
                <div class="card-tooltip-container" data-toggle="tooltip" data-placement="top" v-bind:title="$t('profile_picture_tooltip')">
                    <i class="fas fa-info"></i>
                </div>
                <div class="row">
                    <!-- No preview selected (show current) -->
                    <div class="file-preview no-file-preview" v-if="(currentUser.profile_picture == null || currentUser.profile_picture.length == 0) && previewUrl.length == 0"></div>

                    <div class="file-preview no-file-preview" :style="{'background-image':'url(' + currentUser.profile_picture + ')'}" v-else-if="previewUrl.length == 0"></div>

                    <!-- Preview selected (show preview) -->
                    <div class="file-preview" :style="{'background-image':'url(' + previewUrl + ')'}" v-else-if="previewUrl.length > 0"></div>

                    <div class="file-drop file-drop-preview" @drop.prevent="viaDrop" @dragover.prevent>
                        <div class="file-drop-center">
                            <i class="fas fa-file-upload file-upload-icon"></i>
                            <p>{{$t('drop_a_file_or').charAt(0).toUpperCase() + $t('drop_a_file_or').slice(1)}} <label for="preview-upload" class="pointer">{{$t('click_here')}}</label></p>

                            <!-- <p v-if="uploadResponse == 'invalid'">{{$t('error.invalid_file')}}: <span v-for="item in allowedExtensions">.{{item}} </span></p> -->
                            <p v-if="uploadResponse == 'success'">{{$t('selected').charAt(0).toUpperCase() + $t('selected').slice(1)}}: {{fileName}}</p>
                        </div>
                        <input type="file" id="preview-upload" accept=".svg, .png, .jpg, .jpeg, .gif, .webp, .SVG, .PNG, .JPG, .JPEG, .GIF, .WEBP" @change="viaSelector" style="visibility: hidden; position:absolute;" />
                    </div>


                </div>

                <div class="d-flex flex-row-reverse">
                    <button class="btn btn-save" :disabled="!canSave" v-on:click="save()">{{$t('save').charAt(0).toUpperCase() + $t('save').slice(1)}}</button>
                    <button class="btn btn-cancel mr-2" :disabled="!canSave" v-on:click="cancel()">{{$t('cancel').charAt(0).toUpperCase() + $t('confirm').slice(1)}}</button>
                </div>
            </div>
        </div>
    </div>
</template>

<script>
    export default {
        name: 'ProfileImage',
        data() {
            return {
                uploadResponse: '',
                fileName: '',
                previewUrl: '',

                file: null,

                saved: false,

                allowedExtensions: ['svg', 'png', 'jpg', 'jpeg', 'gif', 'webp', 'SVG', 'PNG', 'JPG', 'JPEG', 'GIF', 'WEBP']
            }
        },
        computed: {
            currentUser(){
                return this.$parent.currentUser;
            },
            canSave(){
                return this.file != null;
            }
        },
        methods: {
            viaSelector(e){
                const file = e.target.files[0];

                this.fileName = file.name;
                this.file = file;
                this.previewUrl = URL.createObjectURL(file);
                this.uploadResponse = "success";
            },
            viaDrop(e){
                let droppedFiles = e.dataTransfer.files;

                if(!droppedFiles) return;

                var extension = droppedFiles[0].name.split('.').splice(-1,1).join('.');
                if(!this.allowedExtensions.includes(extension)){
                    this.uploadResponse = "invalid";
                }else{
                    this.fileName = droppedFiles[0].name;
                    this.file = droppedFiles[0];
                    this.previewUrl = URL.createObjectURL(droppedFiles[0]);
                    this.uploadResponse = "success";
                }
            },
            cancel(){
                this.uploadResponse = '';
                this.fileName = '';
                this.previewUrl = '';
                this.file = null;
                this.saved = false;
            },
            save(){
                let formData = new FormData();
                formData.append('file', this.file);

                this.axiosRequest('post', 'user/updateProfilePicture', formData, {headers: {'Content-Type': 'multipart/form-data'}})
                .then(response => {
                    this.$store.dispatch('getUser')
                    .then(userResponse => {
                        this.$noty.success(this.$t('profile_picture_updated'));
                        this.cancel();
                        this.saved = true;
                    }).catch(err => {
                        console.log(err);
                    });
                })
                .catch(err => {
                    // this.setMessage('error', 'Oops something went wrong', 5);
                    console.log(err);
                });
            },
        },
        mounted(){
        }
    }
</script>
