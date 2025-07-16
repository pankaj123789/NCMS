(function (document, window, index) {
    'use strict';

    // feature detection for drag&drop upload
    var isAdvancedUpload = function () {
        var div = document.createElement('div');
        return (('draggable' in div) || ('ondragstart' in div && 'ondrop' in div)) && 'FormData' in window && 'FileReader' in window;
    }();

    $.fn.dragAndDropUploader = function () {
        var $this = this;
        var forms = $this.closest('form');
        var inForm = forms.length > 0;
        var submitForm = null;

        if (inForm)
            submitForm = forms[0];

        var html = '';

        if (!inForm)
            html += '<form method="post" action="' + this.attr('data-action') + '" enctype="multipart/form-data" novalidate class="js box">';
        else
            html += '<div class="js box">';

        html += '\
<input type="hidden" name="id" value="' + this.attr('data-id') + '" />\
<div class="box__input">\
    <svg class="box__icon" xmlns="http://www.w3.org/2000/svg" width="50" height="43" viewBox="0 0 50 43"><path d="M48.4 26.5c-.9 0-1.7.7-1.7 1.7v11.6h-43.3v-11.6c0-.9-.7-1.7-1.7-1.7s-1.7.7-1.7 1.7v13.2c0 .9.7 1.7 1.7 1.7h46.7c.9 0 1.7-.7 1.7-1.7v-13.2c0-1-.7-1.7-1.7-1.7zm-24.5 6.1c.3.3.8.5 1.2.5.4 0 .9-.2 1.2-.5l10-11.6c.7-.7.7-1.7 0-2.4s-1.7-.7-2.4 0l-7.1 8.3v-25.3c0-.9-.7-1.7-1.7-1.7s-1.7.7-1.7 1.7v25.3l-7.1-8.3c-.7-.7-1.7-.7-2.4 0s-.7 1.7 0 2.4l10 11.6z" /></svg>\
    <input type="file" name="files[]" id="file" class="box__file" data-multiple-caption="{count} files selected" multiple />\
    <label for="file"><strong>Choose a file</strong><span class="box__dragndrop"> or drag it here</span>.</label>\
</div>\
<div class="box__uploading">Uploading&hellip;</div>\
<div class="box__success">Done! <a href="#" class="box__restart" role="button">Upload more?</a></div>\
<div class="box__error">Error! <a href="#" class="box__restart" role="button">Try again!</a></div>';

        if (!inForm)
            html += '</form>';
        else
            html += '</div>';

        var $form = $(html);

        this.append($form);

        var form = $form[0];

        if (!inForm)
            submitForm = form;

        var method = submitForm.getAttribute("method");
        var action = submitForm.getAttribute("action");
        var enctype = submitForm.getAttribute("enctype");

        var input = form.querySelector('input[type="file"]'),
            label = form.querySelector('label'),
            errorMsg = form.querySelector('.box__error'),
            restart = form.querySelectorAll('.box__restart'),
            droppedFiles = false,
            showFiles = function (files) {
                label.textContent = files.length > 1 ? (input.getAttribute('data-multiple-caption') || '').replace('{count}', files.length) : files[0].name;
            },
            triggerFormSubmit = function () {
                submitForm.setAttribute('method', 'post');
                submitForm.setAttribute('action', $this.attr('data-action'));
                submitForm.setAttribute('enctype', 'multipart/form-data');

                var ajaxData = new FormData(submitForm);
                if (droppedFiles) {
                    Array.prototype.forEach.call(droppedFiles, function (file) {
                        ajaxData.append(input.getAttribute('name'), file);
                    });
                }

                // ajax request
                var ajax = new XMLHttpRequest();
                ajax.open(submitForm.getAttribute('method'), submitForm.getAttribute('action'), true);

                ajax.onload = function () {
                    form.classList.remove('is-uploading');
                    resetFormAttributes();
                    if (ajax.status >= 200 && ajax.status < 400) {
                        var data = JSON.parse(ajax.responseText);
                        form.classList.add(data.success == true ? 'is-success' : 'is-error');
                        if (!data.success) {
                            errorMsg.innerHTML = errorMsg.innerHTML.replace("Error", data.error);
                            label.textContent = "Try again!";
                            var childError = errorMsg.firstElementChild;
                            childError.addEventListener('click', function (e) {
                                e.preventDefault();
                                form.classList.remove('is-error', 'is-success');
                                input.click();
                            });
                        }
                        else $this.trigger('uploadsuccess');
                        input.value = '';
                    }
                    else alert('Error. Please, contact the webmaster!');
                };

                ajax.onerror = function () {
                    form.classList.remove('is-uploading');
                    alert('Error. Please, try again!');
                };

                ajax.send(ajaxData);
            },
            resetFormAttributes = function () {
                submitForm.setAttribute('method', method);
                submitForm.setAttribute('action', action);
                submitForm.setAttribute('enctype', enctype);
            };

        // letting the server side to know we are going to make an Ajax request
        var ajaxFlag = document.createElement('input');
        ajaxFlag.setAttribute('type', 'hidden');
        ajaxFlag.setAttribute('name', 'ajax');
        ajaxFlag.setAttribute('value', 1);
        submitForm.appendChild(ajaxFlag);

        // automatically submit the form on file select
        input.addEventListener('change', function (e) {
            showFiles(e.target.files);
            triggerFormSubmit();
        });

        // drag&drop files if the feature is available
        if (isAdvancedUpload) {
            form.classList.add('has-advanced-upload'); // letting the CSS part to know drag&drop is supported by the browser

            ['drag', 'dragstart', 'dragend', 'dragover', 'dragenter', 'dragleave', 'drop'].forEach(function (event) {
                form.addEventListener(event, function (e) {
                    // preventing the unwanted behaviours
                    e.preventDefault();
                    e.stopPropagation();
                });
            });
            ['dragover', 'dragenter'].forEach(function (event) {
                form.addEventListener(event, function () {
                    form.classList.add('is-dragover');
                });
            });
            ['dragleave', 'dragend', 'drop'].forEach(function (event) {
                form.addEventListener(event, function () {
                    form.classList.remove('is-dragover');
                });
            });
            form.addEventListener('drop', function (e) {
                droppedFiles = e.dataTransfer.files; // the files that were dropped
                showFiles(droppedFiles);
                triggerFormSubmit();
            });
        }

        // restart the form if has a state of error/success
        Array.prototype.forEach.call(restart, function (entry) {
            entry.addEventListener('click', function (e) {
                e.preventDefault();
                form.classList.remove('is-error', 'is-success');
                input.click();
            });
        });

        // Firefox focus bug fix for file input
        input.addEventListener('focus', function () { input.classList.add('has-focus'); });
        input.addEventListener('blur', function () { input.classList.remove('has-focus'); });

        return this;
    }
}(document, window, 0));