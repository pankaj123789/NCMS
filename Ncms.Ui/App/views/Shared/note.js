define([
    'modules/common',
    'modules/enums',
    'services/screen/date-service',
    'services/screen/message-service',
    'views/shared/note-document'
], function (common, enums, dateService, messageService, noteDocument) {
    function getInstance() {
        var resumeLength = 400;

        var serverModel = {
            NoteId: ko.observable(),
            Note: ko.observable().extend({ required: true }),
            CreatedDate: ko.observable(),
            UserId: ko.observable(0),
            User: ko.observable()
        };

        var validation = ko.validatedObservable(serverModel);
        var cleanServerModel = ko.toJS(serverModel);

        var vm = {
            allowAttachments: ko.observable(false),
            maxLength: 10000,
            notes: ko.observableArray([]),
            note: serverModel,
            parseNote: function () {
                return ko.toJS(serverModel);
            },
            getNotesPromise: null,
            postNotesPromise: null,
            removeNotesPromise: null,
            noteDocumentInstance: noteDocument.getInstance(),
            enableShowAllNotes: ko.observable(false),
            showAll: ko.observable(false)
        };

        var dirtyFlag = new ko.DirtyFlag([vm.notes], false);
        vm.showAll.subscribe(function() {
            vm.getNotes();
        });

        $.extend(vm, {
            getNotes: function() {
                if (validation.errors) {
                    validation.errors.showAllMessages(false);
                }

                vm.getNotesPromise(vm.showAll()).then(function(data) {
                    vm.notes(ko.utils.arrayMap(data, function(note) {
                        var now = moment();
                        var date = note.ModifiedDate || note.CreatedDate;
                        var momentDate = moment(date);

                        var css = 'tl-wrap';

                        if (now.diff(momentDate, 'days') === 0) {
                            css += ' b-success';
                        } else if (now.diff(momentDate, 'weeks') === 0) {
                            css += ' b-primary';
                        } else if (now.diff(momentDate, 'months') === 0) {
                            css += ' b-info';
                        } else if (now.diff(momentDate, 'years') === 0) {
                            css += ' b-warning';
                        }

                        note.Note = common.functions().htmlEncode(note.Note).replace(/\\n/g, "<br />");

                        note.ReferenceURL = '';
                        note.ReferenceText = '';

                        var noteReferenceType = enums.NoteReferenceType;
                        if (note.ReferenceType === noteReferenceType.Application) {
                            var appText = 'APP' + note.Reference;
                            var appUrl = "#/application/" + note.Reference + "?tab=notes";

                            note.ReferenceURL = appUrl;
                            note.ReferenceText = appText;
                            note.Note = ' ' + note.Note;
                        } if (note.ReferenceType === noteReferenceType.Test) {

                            var testText = 'Attendance ' + note.Reference;
                            var testUrl = "#/test/" + note.Reference + "?tab=notes";
                            
                            note.ReferenceURL = testUrl;
                            note.ReferenceText = testText;
                            note.Note = ' ' + note.Note;
                        }

                        var resume = note.Note.substr(0, resumeLength);
                        var showMore = note.Note.length > resumeLength;

                        return $.extend(note, {
                            css: css,
                            Date: date,
                            DateFormatted: momentDate.format(CONST.settings.shortDateDisplayFormat),
                            title: momentDate.format(CONST.settings.timeDisplayFormat) + ' - ' + note.User,
                            resume: resume,
                            expanded: ko.observable(false),
                            showMore: showMore,
                            text: function () {
                                return this.expanded() ? this.Note : this.resume;
                            },
                            expandCollapseText: function () {
                                return this.expanded() ? ko.Localization('Naati.Resources.Shared.resources.ShowLess') : ko.Localization('Naati.Resources.Shared.resources.ShowMore');
                            },
                            expandCollapse: function () {
                                this.expanded(!this.expanded());
                            },
                            isOwner: note.UserId === currentUser.Id,
                            canEdit: note.UserId === currentUser.Id && currentUser.hasPermissionSync(enums.SecNoun.Notes, enums.SecVerb.Update),
                            canDelete: note.UserId === currentUser.Id && currentUser.hasPermissionSync(enums.SecNoun.Notes, enums.SecVerb.Delete)
                        });
                    }));

                    dirtyFlag().reset();
                });
            },
            postNote: function () {
                if (!validation.isValid()) {
                    validation.errors.showAllMessages();
                    return false;
                }

                var noteLength = vm.note.Note().trim().length;

                if (!noteLength) {
                    return;
                }

                if (noteLength > vm.maxLength) {
                    toastr.error(ko.Localization('Naati.Resources.Validation.resources.MaxLengthExceeded').replace(/\{0\}/, vm.maxLength));
                    return;
                }

                vm.postNotesPromise().then(function () {
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.SavedSuccessfully'));
                    ko.viewmodel.updateFromModel(serverModel, cleanServerModel);
                    vm.getNotes();
                });
            },
            editNote: function (note) {
                ko.viewmodel.updateFromModel(serverModel, note);
            },
            cancelNote: function () {
                ko.viewmodel.updateFromModel(serverModel, cleanServerModel);

                if (validation.errors) {
                    validation.errors.showAllMessages(false);
                }
            },
            tryDeleteNote: function (note) {
                messageService.remove().then(function (answer) {
                    if (answer === 'yes') {
                        vm.deleteNote(note);
                    }
                });
            },
            deleteNote: function (note) {
                vm.removeNotesPromise(note).then(function () {
                    toastr.success(ko.Localization('Naati.Resources.Shared.resources.DeletedSuccessfully'));
                    vm.getNotes();
                });
            },
            addDocument: function (note) {
                vm.noteDocumentInstance.show(note);
            },
            isDirty: function () {
                return dirtyFlag().isDirty();
            },
            humanizeDate: function (date) {
                return common.functions().humanizeDate(moment(date).toDate());
            },
            postNoteText: function () {
                return ko.Localization(serverModel.NoteId() ? 'Naati.Resources.Shared.resources.UpdateNote' : 'Naati.Resources.Shared.resources.AddNote');
            },
            canAddEdit: function () {
                return serverModel.NoteId() ? currentUser.hasPermissionSync(enums.SecNoun.Notes, enums.SecVerb.Update) : currentUser.hasPermissionSync(enums.SecNoun.Notes, enums.SecVerb.Create) ;
            }
        });

        return vm;
    }

    return {
        getInstance: getInstance
    };
});
