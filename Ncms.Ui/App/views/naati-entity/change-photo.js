define([
    'services/servercallbackprocessor',
    'services/personimage-data-service',
    'services/person-data-service',
    'services/screen/message-service'
], function (servercallbackprocessor, personImageService, personService, message) {
    return {
        getInstance: getInstance
    };

    function getInstance(person) {
        var imageLimitMB = 5;
        var vm = {
            person: person,
            newPhotoModalId: 'newPhotoModal',
            dropZoneId: 'newPhotoDropZone',
            formId: 'newPhotoForme',
            showPhoto: ko.observable(false),
            uploadLoadedMessage: ko.observable(),
            uploadRemainingMessage: ko.observable(),
            total: ko.observable(),
            loaded: ko.observable(),
            bitrates: ko.observableArray(),
            percent: ko.observable(),
            isLoading: ko.observable(),
            file: ko.observable(),
            photoUrl: ko.pureComputed(function () {
                person.PhotoDate();//just to force url refresh
                return personImageService.newUrl(person.NaatiNumber());
            })
        };

        vm.loaded.subscribe(function () {
            var bitrateAvg = 0;
            $.each(vm.bitrates(), function (i, e) {
                bitrateAvg += e;
            });

            bitrateAvg = vm.bitrates().length === 0 ? 0 : bitrateAvg / vm.bitrates().length;

            var remaining = bitrateAvg === 0 ? 0 : (vm.total() - vm.loaded()) * 8 / bitrateAvg;
            remaining = moment.duration({ s: remaining }).format('hh:mm:ss');

            var timeParts = getTimeParts(remaining);
            var remainingMessage = getRemainingMessage(timeParts);

            vm.uploadRemainingMessage(uploadRemainingMessage.format(remainingMessage, ko.bytesToSize(bitrateAvg).toLowerCase() + 'ps'));
        });

        var defer = null;
        var $dropZone = null;
        var $form = null;
        var $file = null;
        var uploadLoadedMessage = ko.Localization('Naati.Resources.Shared.resources.UploadLoadedMessage');
        var uploadRemainingMessage = ko.Localization('Naati.Resources.Shared.resources.UploadRemainingMessage');
        var hourText = ko.Localization('Naati.Resources.Shared.resources.Hour');
        var hoursText = ko.Localization('Naati.Resources.Shared.resources.Hours');
        var minuteText = ko.Localization('Naati.Resources.Shared.resources.Minute');
        var minutesText = ko.Localization('Naati.Resources.Shared.resources.Minutes');
        var secondText = ko.Localization('Naati.Resources.Shared.resources.Second');
        var secondsText = ko.Localization('Naati.Resources.Shared.resources.Seconds');

        vm.show = function () {
            vm.showPhoto(true);

            defer = Q.defer();

            $('#' + vm.newPhotoModalId).modal('show');
            vm.file(null);

            if ($dropZone == null) {
                $dropZone = $('#' + vm.dropZoneId);

                $dropZone[0].addEventListener("dragenter", dragenter, false);
                $dropZone[0].addEventListener("dragover", dragover, false);
                $dropZone[0].addEventListener("drop", drop, false);
            }

            $('img', $dropZone).attr('src', null);
            $form = $('#' + vm.formId);
            $('[type=file]', $form).remove();
            $file = $('<input type="file" name="fileupload" accept="image/*" />').appendTo($form);
            $file[0].addEventListener('change', function (e) {
                $form.find('[name=file]').val(this.value);
                handleImage(e.target.files);
            }, false);

            return defer.promise;
        };

        vm.close = function () {
            vm.showPhoto(false);
            $('#' + vm.newPhotoModalId).modal('hide');
        };

        vm.selectPhoto = function () {
            $file[0].click();
        };

        // settable by parent control
        vm.canUpload = function () { return true; };
        vm.upload = function () {
            if (!vm.canUpload()) {
                return;
            }

            var formData = new FormData($form[0]);

            $.ajax({
                type: 'POST',
                url: personImageService.url(),
                data: formData,
                xhr: function () {
                    vm.uploadLoadedMessage(uploadLoadedMessage.format(0, 0));
                    vm.uploadRemainingMessage(uploadRemainingMessage.format("-", "-"));
                    vm.total(0);
                    vm.loaded(0);
                    vm.bitrates([]);
                    vm.percent(0);
                    vm.isLoading(true);

                    var myXhr = $.ajaxSettings.xhr();
                    var startedAt = new Date();

                    if (myXhr.upload) {
                        myXhr.upload.addEventListener('progress', function (e) {
                            if (e.lengthComputable) {
                                var max = e.total;
                                var current = e.loaded;

                                var percentage = (current * 100) / max;

                                vm.uploadLoadedMessage(uploadLoadedMessage.format(ko.bytesToSize(e.loaded), ko.bytesToSize(e.total)));
                                vm.uploadRemainingMessage(uploadRemainingMessage.format("-", "-"));

                                vm.percent(parseInt(percentage));
                                vm.loaded(e.loaded);
                                vm.total(e.total);

                                var secondsElapsed = (new Date().getTime() - startedAt.getTime()) / 1000;
                                var bytesPerSecond = secondsElapsed ? e.loaded / secondsElapsed : 0;

                                vm.bitrates.push(bytesPerSecond);
                            }
                        }, false);
                    }
                    return myXhr;
                },
                success: function () {
                    vm.isLoading(false);
                    defer.resolve();
                },
                cache: false,
                contentType: false,
                processData: false,
                error: function (XMLHttpRequest, textStatus, errorThrown) {
                    vm.isLoading(false);
                    servercallbackprocessor.showError(XMLHttpRequest, textStatus, errorThrown);
                }
            });
        };

        function getTimeParts(time) {
            var tmp = time.split(':');
            return {
                second: parseInt(tmp.pop()),
                minute: tmp.length > 0 ? parseInt(tmp.pop()) : 0,
                hour: tmp.length > 0 ? parseInt(tmp.pop()) : 0
            };
        }

        function getPartMessage(timePart, singularText, pluralText) {
            if (timePart == 0) {
                return '';
            }

            if (timePart == 1) {
                return timePart + ' ' + singularText;
            }

            return timePart + ' ' + pluralText;
        }

        function getRemainingMessage(timeParts) {
            return getPartMessage(timeParts.hour, hourText, hoursText) + ' ' +
                getPartMessage(timeParts.minute, minuteText, minutesText) + ' ' +
                getPartMessage(timeParts.second, secondText, secondsText);
        }

        var preventHandleImage = false;
        function handleImage(files) {
            if (preventHandleImage) return;

            var reader = new FileReader();
            reader.onload = function (event) {
                $('img', $dropZone).attr('src', event.target.result);
            }

            if (files.length && files[0].type.indexOf('image') == 0) {
                if (files[0].size <= (imageLimitMB * 1024 * 1024)) {
                    vm.file(true);
                    reader.readAsDataURL(files[0]);
                }
                else {
                    message.alert({
                        title: "Upload Failed",
                        content: "You cannot upload a photo larger than " + imageLimitMB + "MB."
                    });
                }
            }
        }

        function dragenter(e) {
            e.stopPropagation();
            e.preventDefault();
        }

        function dragover(e) {
            e.stopPropagation();
            e.preventDefault();
        }

        function drop(e) {
            e.stopPropagation();
            e.preventDefault();
            //you can check e's properties
            //console.log(e);
            var dt = e.dataTransfer;
            var files = dt.files;

            //this code line fires your 'handleImage' function (imageLoader change event)
            preventHandleImage = true;
            $file[0].files = files;
            preventHandleImage = false;

            handleImage(files);
        }

        return vm;
    }
});