define(['services/queued-query-requester', 'services/requester'],
    function(queuedQueryRequester, requester) {

        var queuedQueryService = queuedQueryRequester('credentialapplication');
        var service = requester('credentialapplication');

        function QueuedResource(queuedQueryService, service) {

            this.save = function save(request) {
                return queuedQueryService.post(request, 'save');
            }

            this.forms = function forms() {
                return service.getFluid('forms');
            }

            this.submit = function submit(request) {
                return queuedQueryService.post(request, 'submit');
            }

            this.remove = function remove(request) {
                return queuedQueryService.post(request, 'delete');
            }

            this.sections = function sections(request) {
                return service.getFluid('sections', request);
            }


            this.lookup = function lookup(action, request) {
                return service.getFluid(action, request);
            }

            this.credentials = function credentials(request) {
                return queuedQueryService.getFluid('credentials', request);
            }

            this.categories = function categories(request) {
                return queuedQueryService.getFluid('categories', request);
            }


            this.availableCredentials = function categories(request) {
                return queuedQueryService.getFluid('availablecredentials', request);
            }

            this.additionalSkills = function categories(request) {
                return queuedQueryService.getFluid('additionalskills', request);
            }

            this.fees = function fees(request) {
                return queuedQueryService.getFluid('fees', request);
            }

            this.deleteCredential = function deleteCredential(request) {
                return queuedQueryService.post(request, 'deletecredential');
            }

            this.createCredential = function createCredential(request) {
                return queuedQueryService.post(request, 'credential');
            }

            this.levels = function levels(request) {
                return service.getFluid('levels', request);
            }

            this.skills = function skills(request) {
                return service.getFluid('skills', request);
            }

            this.documentTypes = function documentTypes(request) {
                return queuedQueryService.getFluid('documenttypes', request);
            }

            this.customerDetails = function customerDetails(request) {
                return queuedQueryService.getFluid('customerdetails', request);
            }

            this.personDetails = function personDetails(request) {
                return queuedQueryService.getFluid('persondetails', request);
            }

            this.testSessions = function testSessions(request) {
                return queuedQueryService.getFluid('testsessions', request);
			}

			this.countries = function countries() {
				return queuedQueryService.getFluid('countries');
			}

			this.pdPointsMet = function pdPointsMet(request) {
				return queuedQueryService.getFluid('pdPointsMet', request);
			}

			this.workPracticeMet = function workPracticeMet(request) {
				return queuedQueryService.getFluid('workPracticeMet', request);
			}

			this.getRecertificaitonOptions = function getRecertificaitonOptions(request) {
				return queuedQueryService.getFluid('getRecertificaitonOptions', request);
			}

			this.replaceTokens = function replaceTokens(request) {
				return queuedQueryService.getFluid('replaceTokens', request);
			}

            this.personPhoto = function personPhoto(request) {
                return service.getFluid('personphoto', request);
            }

            this.personTitles = function personTitles() {
                return queuedQueryService.getFluid('personTitles');
            }

            this.personVerification = function personVerification(request) {
                return queuedQueryService.post(request, 'personverification');
            }

            this.customerApplication = function customerApplication(request) {
                return queuedQueryService.getFluid('customerapplication', request);
            }

            this.canEditPersonDetails = function canEditPersonDetails() {
                return queuedQueryService.getFluid('canEditPersonDetails');
            }

            this.documents = function documents(request) {
               return queuedQueryService.getFluid('documents', request);
            }

            this.deleteDocument = function deleteDocument(request) {
                return queuedQueryService.post(request, 'deletedocument');
            }

            this.parseAddress = function parseAddress(request) {
                return service.post(request, 'parseaddress');
            }

            this.createCredentialApplication = function createCredentialApplication(request) {
                return queuedQueryService.post(request, 'createcredentialapplication');
			}

			this.credentialRequestLimit = function () {
				return queuedQueryService.getFluid('credentialrequestlimit');
			}

			this.inProgressCredentialsRequests = function (request) {
				return queuedQueryService.getFluid('getInProgressCredentialsRequests', request);
			}

			this.activeAndFutureCredentials = function (request) {
				return queuedQueryService.getFluid('getActiveAndFutureCredentials', request);
			}
        }

        return new QueuedResource(queuedQueryService, service);
    });