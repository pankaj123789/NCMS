define(['modules/enums'],
	function (enums) {
		

	    var forms = [
            {
                name: 'Person',
                title: ko.Localization('Naati.Resources.Shared.resources.Calendar'),
                ico: 'fa fa-calendar',
                showInMenu: true,
                route: 'calendar',
                moduleId: 'views/calendar/calendar-index',
                secNoun: enums.SecNoun.TestSession,
                secVerb: enums.SecVerb.Read
            },
            {
                name: 'parentPeopleAndOrganisations',
                title: ko.Localization('Naati.Resources.Shared.resources.PeopleAndOrganisations'),
                ico: 'fa fa-users',
                showInMenu: true,
                menus: [
                    {
                        name: 'Person',
                        title: ko.Localization('Naati.Resources.Shared.resources.People'),
                        route: 'person',
                        moduleId: 'views/person/person-search',
                        ico: 'fa fa-user',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PeopleAndOrganisations') },
                        secNoun: enums.SecNoun.Person,
                        secVerb: enums.SecVerb.Read
                    },
                    {
                        name: 'Organisation',
                        title: ko.Localization('Naati.Resources.Shared.resources.Organisations'),
                        route: 'organisation',
                        moduleId: 'views/institution/institution-search',
                        ico: 'fa fa-sitemap',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PeopleAndOrganisations') },
                        secNoun: enums.SecNoun.Organisation,
                        secVerb: enums.SecVerb.Read
                    },
                    {
                        name: 'EndorsedQualifications',
                        title: ko.Localization('Naati.Resources.Shared.resources.EndorsedQualifications'),
                        route: 'endorsed-qualification',
                        moduleId: 'views/endorsed-qualification/endorsed-qualification-search',
                        ico: 'fa fa-graduation-cap',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PeopleAndOrganisations') },
                        secNoun: enums.SecNoun.EndorsedQualification,
                        secVerb: enums.SecVerb.Read
                    }
                ]
            },
            {
                name: 'parentApplication',
                title: ko.Localization('Naati.Resources.Shared.resources.Applications'),
                ico: 'fa fa-check-square',
                showInMenu: true,
                menus: [
                    {
                        name: 'Application',
                        title: ko.Localization('Naati.Resources.Shared.resources.Applications'),
                        route: 'application',
                        moduleId: 'views/application/application-search',
                        ico: 'fa fa-check-circle',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Applications') },
                        secNoun: enums.SecNoun.Application,
                        secVerb: enums.SecVerb.Read
                    },
                    {
                        name: 'Credential',
                        title: ko.Localization('Naati.Resources.Shared.resources.ProductsExport'),
                        route: 'products-export',
                        moduleId: 'views/application/products-export',
                        ico: 'fa fa-id-card',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Applications') },
                        secNoun: enums.SecNoun.Credential,
                        secVerb: enums.SecVerb.Search
                    },
                    {
                        name: 'Application',
                        title: ko.Localization('Naati.Resources.Shared.resources.RequestSummary'),
                        route: 'credential-request-summary',
                        moduleId: 'views/credential-request/credential-request-summary',
                        ico: 'fa fa-circle-notch',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Applications') },
                        secNoun: enums.SecNoun.CredentialRequest,
                        secVerb: enums.SecVerb.Read
                    }
                ]
            },
            {
                name: 'parentTest',
                title: ko.Localization('Naati.Resources.Shared.resources.Testing'),
                ico: 'glyphicon glyphicon-list-alt',
                showInMenu: true,
                menus: [
                    {
                        name: 'TestSitting',
                        title: ko.Localization('Naati.Resources.Test.resources.Tests'),
                        route: 'test',
                        moduleId: 'views/test/index',
                        ico: 'glyphicon glyphicon-list-alt',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Test.resources.Tests') },
                        secNoun: enums.SecNoun.TestSitting,
                        secVerb: enums.SecVerb.Read
                    },
                    {
                        name: 'TestSession',
                        title: ko.Localization('Naati.Resources.Shared.resources.Sessions'),
                        ico: 'glyphicon glyphicon-text-width',
                        showInMenu: true,
                        route: 'test-session',
                        moduleId: 'views/test-session/test-session-search',
                        secNoun: enums.SecNoun.TestSession,
                        secVerb: enums.SecVerb.Read
                    },
                    {
                        name: 'TestMaterial',
                        title: ko.Localization('Naati.Resources.Shared.resources.TestMaterial'),
                        ico: 'glyphicon glyphicon-pencil',
                        showInMenu: true,
                        route: 'test-material',
                        moduleId: 'views/test-material/test-material-search',
                        secNoun: enums.SecNoun.TestMaterial,
                        secVerb: enums.SecVerb.Manage
                    },
                    {
                        name: 'TestSpecification',
                        title: ko.Localization('Naati.Resources.Shared.resources.TestSpecifications'),
                        ico: 'icon-notebook',
                        showInMenu: true,
                        route: 'test-specification',
                        moduleId: 'views/test-specification/test-specification-search',
                        secNoun: enums.SecNoun.TestSpecification,
                        secVerb: enums.SecVerb.Manage
                    },
                    {
                        name: 'TestMaterial',
                        title: ko.Localization('Naati.Resources.Shared.resources.TestMaterialRequests'),
                        ico: 'far fa-newspaper',
                        showInMenu: true,
                        route: 'test-material-request-search',
                        moduleId: 'views/test-material/request/test-material-request-search',
                        secNoun: enums.SecNoun.TestMaterial,
                        secVerb: enums.SecVerb.Manage

                    }
                ]
            },
            {
                name: 'parentPayRuns',
                title: ko.Localization('Naati.Resources.Shared.resources.PayRuns'),
                ico: 'fa fa-credit-card',
                showInMenu: true,
                menus: [
                    {
                        name: 'PayRun',
                        secVerb: enums.SecVerb.Validate,
                        secNoun: enums.SecNoun.PayRun,
                        title: ko.Localization('Naati.Resources.Shared.resources.ValidateForPayRun'),
                        route: 'payroll/validate',
                        moduleId: 'views/payroll/payroll-validate',
                        ico: 'fa fa-check-square',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PayRuns') }
                    },
                    {
                        name: 'PayRun',
                        secVerb: enums.SecVerb.Create,
                        secNoun: enums.SecNoun.PayRun,
                        title: ko.Localization('Naati.Resources.Shared.resources.NewPayRun'),
                        route: 'payroll/new',
                        moduleId: 'views/payroll/payroll-new',
                        ico: 'fa fa-plus-square',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PayRuns') }
                    },
                    {
                        name: 'PayRun',
                        secVerb: enums.SecVerb.Finalise,
                        secNoun: enums.SecNoun.PayRun,
                        title: ko.Localization('Naati.Resources.Shared.resources.InProgressPayRun'),
                        route: 'payroll/in-progress',
                        moduleId: 'views/payroll/payroll-in-progress',
                        ico: 'icon-list',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PayRuns') }
                    },
                    {
                        name: 'PayRun',
                        title: ko.Localization('Naati.Resources.Shared.resources.PreviousPayRun'),
                        route: 'payroll/previous',
                        moduleId: 'views/payroll/payroll-previous',
                        ico: 'fa fa-history',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PayRuns') },
                        secVerb: enums.SecVerb.Manage,
                        secNoun: enums.SecNoun.PayRun,
                    },
                    {
                        name: 'PayRun',
                        title: ko.Localization('Naati.Resources.Shared.resources.PaymentApprovalForMaterialCreation'),
                        route: 'payroll/payment-approval-material-creation',
                        moduleId: 'views/payroll/payroll-material-request',
                        ico: 'far fa-newspaper',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PayRuns') },
                        secVerb: enums.SecVerb.Approve,
                        secNoun: enums.SecNoun.PayRun,
                    },
                    {
                        name: 'PayRun',
                        title: ko.Localization('Naati.Resources.Shared.resources.UpdateTestMaterialCreationPayment'),
                        route: 'payroll/update-test-material-creation-payment',
                        moduleId: 'views/payroll/update-test-material-creation-payment',
                        ico: 'glyphicon glyphicon-pencil',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.PayRuns') },
                        secVerb: enums.SecVerb.Update,
                        secNoun: enums.SecNoun.PayRun,
                    }
                ]
            },
            {
                name: 'parentFinance',
                title: ko.Localization('Naati.Resources.Shared.resources.Finance'),
                ico: 'fa fa-money-bill-alt',
                showInMenu: true,
                menus: [
                    //{
                    //    name: 'Invoice',
                    //    title: ko.Localization('Naati.Resources.Shared.resources.NewInvoice'),
                    //    route: 'finance/new-invoice',
                    //    moduleId: 'views/finance/new-invoice',
                    //    ico: 'fa fa-list',
                    //    showInMenu: true,
                    //    menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Finance') },
                    //    secVerb: enums.SecVerb.Create,
                    //    secNoun: enums.SecNoun.Invoice,
                    //},
                    //{
                    //    name: 'Payment',
                    //    title: ko.Localization('Naati.Resources.Shared.resources.NewPayment'),
                    //    route: 'finance/new-payment',
                    //    moduleId: 'views/finance/new-payment',
                    //    ico: 'fa fa-dollar-sign',
                    //    showInMenu: true,
                    //    menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Finance') },
                    //    secVerb: enums.SecVerb.Create,
                    //    secNoun: enums.SecNoun.Payment,
                    //},
                    //{
                    //    name: 'Payment',
                    //    title: ko.Localization('Naati.Resources.Shared.resources.EndOfPeriod'),
                    //    route: 'finance/end-of-period',
                    //    moduleId: 'views/finance/end-of-period',
                    //    ico: 'icon icon-calendar',
                    //    showInMenu: true,
                    //    menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Finance') },
                    //    secVerb: enums.SecVerb.Manage,
                    //    secNoun: enums.SecNoun.Payment,
                    //},
                    {
                        name: 'FinanceOther',
                        title: ko.Localization('Naati.Resources.Shared.resources.FinanceQueue'),
                        route: 'finance/finance-queue',
                        moduleId: 'views/finance/finance-queue',
                        ico: 'glyphicon glyphicon-tasks',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Finance') },
                        secVerb: enums.SecVerb.Send,
                        secNoun: enums.SecNoun.FinanceOther,
                    },
                    {
                        name: 'Payment',
                        title: ko.Localization('Naati.Resources.Shared.resources.RefundApproval'),
                        route: 'finance/refund-approval',
                        moduleId: 'views/finance/refund-approval',
                        ico: 'fa fa-hand-holding-usd',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Finance') },
                        secNoun: enums.SecNoun.CredentialRequest,
                        secVerb: enums.SecVerb.ApproveRefund
                    }
                    //{ // Todo: Backend needs to be fixed
                    //    name: 'Finance',
                    //    title: ko.Localization('Naati.Resources.Shared.resources.CheckApplicationPayments'),
                    //    ico: 'fa fa-dollar',
                    //    showInMenu: true,
                    //    menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.Finance') },
                    //    click: updateInvoices,
                    //    showWithPermission: 'Finance'
                    //}
                ]
            },
            {
                name: 'Panel',
                title: ko.Localization('Naati.Resources.Panel.resources.Panels'),
                route: 'panel',
                moduleId: 'views/panels/index',
                ico: 'glyphicon glyphicon-th',
                showInMenu: true,
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Panel,
            },
            {
                name: 'parentSystem',
                title: ko.Localization('Naati.Resources.Shared.resources.System'),
                ico: 'glyphicon glyphicon-hdd',
                showInMenu: true,
                menus: [
                    {
                        name: 'Email',
                        title: ko.Localization('Naati.Resources.Shared.resources.EmailQueue'),
                        route: 'system/email-queue',
                        moduleId: 'views/email/email-queue',
                        ico: 'fa fa-clone',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Manage,
                        secNoun: enums.SecNoun.Email,
                    },
                    {
                        name: 'EmailTemplate',
                        title: ko.Localization('Naati.Resources.Shared.resources.EmailTemplates'),
                        route: 'system/email-template',
                        moduleId: 'views/email/template/search',
                        ico: 'fa fa-envelope',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Read,
                        secNoun: enums.SecNoun.EmailTemplate,
                    },
                    {
                        name: 'Language',
                        title: ko.Localization('Naati.Resources.Shared.resources.Languages'),
                        ico: 'fa fa-language',
                        route: 'system/language',
                        moduleId: 'views/system/language-search',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Manage,
                        secNoun: enums.SecNoun.Language,
                    },
                    {
                        name: 'Skill',
                        title: ko.Localization('Naati.Resources.Shared.resources.Skills'),
                        ico: 'fa fa-tasks',
                        route: 'system/skill',
                        moduleId: 'views/system/skill-search',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Manage,
                        secNoun: enums.SecNoun.Skill,
                    },
                    {
                        name: 'Venue',
                        title: ko.Localization('Naati.Resources.Shared.resources.Venues'),
                        ico: 'fa fa-university',
                        route: 'system/venue',
                        moduleId: 'views/system/venue-search',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Manage,
                        secNoun: enums.SecNoun.Venue,
                    },
                    {
                        name: 'Location',
                        title: ko.Localization('Naati.Resources.Shared.resources.Locations'),
                        ico: 'fa fa-map-marker',
                        route: 'system/location',
                        moduleId: 'views/system/location-search',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Manage,
                        secNoun: enums.SecNoun.Location,
                    },
                    {
                        name: 'ApiAdmin',
                        title: ko.Localization('Naati.Resources.Shared.resources.ApiAdmin'),
                        ico: 'fas fa-key',
                        route: 'system/apiadmin',
                        moduleId: 'views/system/apiadmin-search',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Create,
                        secNoun: enums.SecNoun.ApiAdministrator,
                    },
                    {
                        name: 'User',
                        title: ko.Localization('Naati.Resources.Shared.resources.Users'),
                        route: 'system/user',
                        moduleId: 'views/system/users-search',
                        ico: 'fa fa-users',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Manage,
                        secNoun: enums.SecNoun.User,
                    },
                    {
                        name: 'System',
                        title: ko.Localization('Naati.Resources.Shared.resources.Setup'),
                        route: 'system/setup',
                        moduleId: 'views/system/setup',
                        ico: 'fa fa-cogs',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.Manage,
                        secNoun: enums.SecNoun.System,
                    },
                    {
                        name: 'Utility',
                        title: ko.Localization('Naati.Resources.Shared.resources.Utility'),
                        route: 'system/utility',
                        moduleId: 'views/system/utility',
                        ico: 'fa fa-tools',
                        showInMenu: true,
                        menuGroup: { name: ko.Localization('Naati.Resources.Shared.resources.System') },
                        secVerb: enums.SecVerb.AssignPastSession,
                        secNoun: enums.SecNoun.Application,
                    }
                ]
            },
	        {
	            route: ['', 'home'],
	            moduleId: 'views/home/home',
	            title: 'Home'
	        },
	        {
	            route: 'search/result/:query',
	            moduleId: 'views/search/result',
	            title: 'Result'
	        },
	        {
	            route: 'system/info',
	            moduleId: 'views/system/system-info',
	            title: 'System Info'
	        },
	        {
	            route: 'system/info/(:test)',
	            moduleId: 'views/system/system-info',
	            title: 'System Info'
            },
            {
                route: 'system/utility/assign-past-test-session-form',
                moduleId: 'views/system/utility-parts/assign-past-test-session-form',
                title: 'Assign to Past Test Session'
            },
            {
                route: 'system/utility/progress-credit-note-form',
                moduleId: 'views/system/utility-parts/progress-credit-note-form',
                title: 'Progress Credit Note'
            },
            {
                route: 'system/utility/progress-invoice-form',
                moduleId: 'views/system/utility-parts/progress-invoice-form',
                title: 'Progress Invoice'
            },
	        {
	            route: 'person/:id',
	            moduleId: 'views/person/person-main',
                title: 'Person',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Person
	        },
	        {
	            route: 'person/:id/settings',
	            moduleId: 'views/person/person-settings',
                title: 'Person Settings',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Person
	        },
	        {
	            route: 'person/:id/archivehistory',
	            moduleId: 'views/person/person-archive-history',
                title: 'Person Archive History',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Person
	        },
	        {
	            route: 'person/:id/audithistory',
	            moduleId: 'views/person/person-audit-history',
                title: 'Person Audit History',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Audit
	        },
	        {
	            route: 'person/:id/addname',
	            moduleId: 'views/person/person-add-name',
                title: 'Person Add Name',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Person
	        },
	        {
	            route: 'person/:id/namehistory',
	            moduleId: 'views/person/person-name-history',
                title: 'Person Name History',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Person
	        },
	        {
	            route: 'address/:entityId/:addressId',
	            moduleId: 'views/naati-entity/address-edit',
                title: 'Address',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'person/:id/address',
	            moduleId: 'views/naati-entity/address-edit',
                title: 'Address',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'phone/:entityId/:phoneId',
	            moduleId: 'views/naati-entity/phone-edit',
                title: 'Phone',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'person/:id/phone',
	            moduleId: 'views/naati-entity/phone-edit',
                title: 'Phone',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'email/:entityId/:emailId',
	            moduleId: 'views/naati-entity/email-edit',
                title: 'Email',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'person/:id/email',
	            moduleId: 'views/naati-entity/email-edit',
                title: 'Email',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'website/:entityId',
	            moduleId: 'views/naati-entity/website-edit',
                title: 'Website',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'person/:id/certification-periods',
	            moduleId: 'views/naati-entity/change-certification-periods',
                title: ko.Localization('Naati.Resources.Person.resources.CertificationPeriods'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.CertificationPeriod

	        },
	        {
	            route: 'test/:id',
	            moduleId: 'views/test/edit',
                title: 'Test Details',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.TestSitting
	        },
	        {
	            route: 'test/:id/audit-history',
	            moduleId: 'views/test/test-audit-log',
                title: 'Test Audit History',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Audit
	        },
	        {
	            route: 'panel(/:panelId)',
	            moduleId: 'views/panels/edit',
                title: 'Panel',
                secVerb: enums.SecVerb.Create,
                secNoun: enums.SecNoun.Panel
	        },
	        {
	            route: 'panel/:panelId/membership(/:panelMembershipId)',
	            moduleId: 'views/panels/edit-member',
                title: 'Panel Membership',
                secVerb: enums.SecVerb.Edit,
                secNoun: enums.SecNoun.Panel
	        },
	        {
	            route: 'materialreq(/:jobId)',
	            moduleId: 'views/test-material/request/edit/index',
                title: 'Material Request',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.MaterialRequest
	        },
	        {
	            route: 'finance/new-payment/:entityId',
	            moduleId: 'views/finance/new-payment',
                title: ko.Localization('Naati.Resources.Shared.resources.NewPayment'),
                secVerb: enums.SecVerb.Create,
                secNoun: enums.SecNoun.Payment
	        },
	        {
	            route: 'finance/new-invoice/:naatiNumber',
	            moduleId: 'views/finance/new-invoice',
                title: ko.Localization('Naati.Resources.Shared.resources.NewInvoice'),
                secVerb: enums.SecVerb.Create,
                secNoun: enums.SecNoun.Invoice
	        },
	        {
	            route: 'application/:applicationId(/:naatiNumber)',
	            moduleId: 'views/application/application-edit',
                title: ko.Localization('Naati.Resources.Shared.resources.Application'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Application
	        },
	        {
	            route: 'application-wizard/:applicationId/:action(/:credentialRequestId)',
	            moduleId: 'views/application/application-wizard',
                title: ko.Localization('Naati.Resources.Shared.resources.Application'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Application
	        },
	        {
	            route: 'organisation/:id',
	            moduleId: 'views/institution/institution-main',
                title: 'Organisation',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Organisation
	        },
	        {
	            route: 'organisation/:id/address',
	            moduleId: 'views/naati-entity/address-edit',
                title: 'Address',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Organisation
	        },
	        {
	            route: 'organisation/:id/phone',
	            moduleId: 'views/naati-entity/phone-edit',
                title: 'Phone',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Organisation
	        },
	        {
	            route: 'organisation/:id/email',
	            moduleId: 'views/naati-entity/email-edit',
                title: 'Email',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Organisation
	        },
	        {
	            route: 'organisation/:id/website',
	            moduleId: 'views/naati-entity/website-edit',
                title: 'Website',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Organisation
	        },
	        {
	            route: 'organisation/:id/settings',
	            moduleId: 'views/institution/institution-settings',
                title: 'Organisation Settings',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Organisation
	        },
	        {
	            route: 'organisation/:id/addname',
	            moduleId: 'views/institution/institution-add-name',
                title: 'Organisation Add Name',
                secVerb: enums.SecVerb.Manage,
                secNoun: enums.SecNoun.Organisation
	        },
	        {
	            route: 'organisation/:id/namehistory',
	            moduleId: 'views/institution/institution-name-history',
                title: 'Organisation Name History',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Organisation
	        },
	        {
	            route: 'organisation/:id/audithistory',
	            moduleId: 'views/institution/institution-audit-history',
                title: 'Organisation Audit History',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Audit
	        },
	        {
	            route: 'add-contact-person/:institutionId',
	            moduleId: 'views/naati-entity/add-contact-person',
                title: 'Add Contact Person',
                secVerb: enums.SecVerb.Create,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'add-contact-person/:institutionId/:contactPersonId',
	            moduleId: 'views/naati-entity/add-contact-person',
                title: 'Add Contact Person',
                secVerb: enums.SecVerb.Create,
                secNoun: enums.SecNoun.Contact
	        },
	        {
	            route: 'test-session/:Id',
	            moduleId: 'views/test-session/manage-test-session',
                title: 'Manage Test Session',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.TestSession
	        },
	        {
	            route: 'test-session/test-sittings/:Id',
	            moduleId: 'views/test-session/test-sitting',
                title: 'Manage Test Sitting',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.TestSitting
	        },
	        {
	            route: 'test-session-wizard(/:testSessionId)',
	            moduleId: 'views/test-session/test-session-wizard',
                title: 'Test Session Details',
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.TestSession
	        },
	        {
	            route:
	                'credential-request-wizard/:credentialApplicationTypeId/:credentialTypeId/:skillId/:credentialRequestStatusTypeId/:testLocationId/:action',
	            moduleId: 'views/credential-request/credential-request-wizard',
                title: ko.Localization('Naati.Resources.Shared.resources.CredentialSummary'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.CredentialRequest
	        },
	        {
	            route: 'test-material/:id',
	            moduleId: 'views/test-material/test-material-edit',
                title: ko.Localization('Naati.Resources.Shared.resources.TestMaterial'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.TestMaterial
	        },
	        {
	            route: 'endorsed-qualification/:id',
	            moduleId: 'views/endorsed-qualification/endorsed-qualification-edit',
                title: ko.Localization('Naati.Resources.Shared.resources.EndorsedQualification'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.EndorsedQualification
	        },                
			{
				route: 'test-specification/:id',
				moduleId: 'views/test-specification/test-specification-edit',
                title: ko.Localization('Naati.Resources.Shared.resources.TestSpecification'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.TestSpecification
			},
			{
				route: 'test-rubric-marks/:jobExaminerId',
				moduleId: 'views/test/test-rubric-marks',
                title: ko.Localization('Naati.Resources.Test.resources.EditRubricResult'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.RubricResult
			},
			{
				route: 'test-rubric-final/:testResultId',
				moduleId: 'views/test/test-rubric-final',
                title: ko.Localization('Naati.Resources.Test.resources.FinalMarksRubricResult'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.RubricResult
			},
			{
				route: 'email-message/:emailMessageId',
                moduleId: 'views/shared/email-message',
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Email
			},
			{
				route: 'system/user/:id',
				moduleId: 'views/system/user-edit',
                title: ko.Localization('Naati.Resources.Shared.resources.UserEdit'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.User
			},
			{
				route: 'system/email-template/:id',
				moduleId: 'views/email/template/email-template',
                title: ko.Localization('Naati.Resources.Shared.resources.Email'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.EmailTemplate
			},
			{
				route: 'test-material-wizard/:id',
				moduleId: 'views/test-material/test-material-wizard',
                title: ko.Localization('Naati.Resources.TestMaterial.resources.AssignTestMaterialWizard'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.TestMaterial
            },
            {
				route: 'logbook/:id',
                moduleId: 'views/logbook/logbook-index',
                title: ko.Localization('Naati.Resources.Person.resources.ViewLogbook'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Logbook
            },
            {
                route: 'logbook/:id/credential/:id',
				moduleId: 'views/logbook/logbook-credential',
                title: ko.Localization('Naati.Resources.Shared.resources.Credential'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Logbook
            },
			{
				route: 'logbook/:id/credential/:id/:id',
				moduleId: 'views/logbook/logbook-credential',
                title: ko.Localization('Naati.Resources.Shared.resources.Credential'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.Logbook
			},
			{
				route: 'logbook/:naatiNumber/professional-development(/:certificationPeriodId)',
				moduleId: 'views/logbook/logbook-professional-development',
                title: ko.Localization('Naati.Resources.Person.resources.ProfessionalDevelopment'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.CertificationPeriod
			},
			{
				route: 'system/skill/:id',
				moduleId: 'views/system/skill-edit',
                title: ko.Localization('Naati.Resources.Skill.resources.EditSkill'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Skill
			},
			{
				route: 'system/language/:id',
				moduleId: 'views/system/language-edit',
                title: ko.Localization('Naati.Resources.Language.resources.EditLanguage'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Language
			},
			{
				route: 'system/venue/:id',
				moduleId: 'views/system/venue-edit',
                title: ko.Localization('Naati.Resources.Venue.resources.EditVenue'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Venue
            },
            {
                route: 'system/location/:id',
                moduleId: 'views/system/location-edit',
                title: ko.Localization('Naati.Resources.Location.resources.EditLocation'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.Location
            },
            {
                route: 'system/apiadmin/:id',
                moduleId: 'views/system/apiadmin-edit',
                title: ko.Localization('Naati.Resources.Api.resources.EditApiAdmin'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.ApiManager
            },
			{
                route: 'test-session/:id/allocate-role-players-wizard',
				moduleId: 'views/test-session/allocate-role-players-wizard',
                title: ko.Localization('Naati.Resources.TestSession.resources.AllocateRolePlayers'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.RolePlayer
            },
            {
                route: 'test-session/:id/view-role-players',
                moduleId: 'views/test-session/view-role-players',
                title: ko.Localization('Naati.Resources.TestSession.resources.ViewRolePlayers'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.RolePlayer
            },
            {
                route: 'test-session/:id/manage-role-players',
                moduleId: 'views/test-session/manage-role-players',
                title: ko.Localization('Naati.Resources.TestSession.resources.ManageRolePlayers'),
                secVerb: enums.SecVerb.Manage,
                secNoun: enums.SecNoun.RolePlayer
            },
            {
                route: 'system/setup/:message',
                moduleId: 'views/system/setup',
                title: ko.Localization('Naati.Resources.Shared.resources.Setup'),
                secVerb: enums.SecVerb.Manage,
                secNoun: enums.SecNoun.System
            },
            {
                route: 'rubric-configuration/question-pass-rule/:testSpecificationId',
                moduleId: 'views/rubric-configuration/rubric-question-pass-rules',
                title: ko.Localization('Naati.Resources.RubricConfiguration.resources.QuestionPassRules'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.RubricResult
            },
            {
                route: 'rubric-configuration/test-band-rule/:testSpecificationId',
                moduleId: 'views/rubric-configuration/rubric-test-band-rules',
                title: ko.Localization('Naati.Resources.RubricConfiguration.resources.TestBandRules'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.RubricResult
            },
            {
                route: 'rubric-configuration/test-question-rule/:testSpecificationId',
                moduleId: 'views/rubric-configuration/rubric-test-question-rules',
                title: ko.Localization('Naati.Resources.RubricConfiguration.resources.TestQuestionRules'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.RubricResult
            },
            {
                route: 'rubric-configuration/marking-band/:rubricMarkingBandId',
                moduleId: 'views/rubric-configuration/marking-band',
                title: ko.Localization('Naati.Resources.RubricConfiguration.resources.MarkingBand'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.RubricResult
            },
            {
                route: 'rubric-configuration/:testSpecificationId',
                moduleId: 'views/rubric-configuration/rubric-configuration-index',
                title: ko.Localization('Naati.Resources.Shared.resources.RubricConfiguration'),
                secVerb: enums.SecVerb.Read,
                secNoun: enums.SecNoun.RubricResult
            },
            {
                route: 'material-request-wizard/:action/:panelId(/:materialRequestId)(/:materialRoundId)',
                moduleId: 'views/material-request/material-request-wizard',
                title: ko.Localization('Naati.Resources.Shared.resources.MaterialRequest'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.MaterialRequest
            },
            {
                route: 'material-request/:materialRequestId',
                moduleId: 'views/material-request/material-request-edit',
                title: ko.Localization('Naati.Resources.Shared.resources.MaterialRequest'),
                secVerb: enums.SecVerb.Update,
                secNoun: enums.SecNoun.MaterialRequest
            }
        ];

		function get(name) {
			var form = $.grep(forms, function (f) {
				return f.name === name;
			});
			
			if (!form || form.length === 0) {
				return null;
			}

			return form[0];
		}

		function list() {
			return forms;
		}

		
		var templateLoaded = [];
		function loadIfNever(template) {
			var defer = Q.defer();

			var isLoaded = ko.utils.arrayFirst(templateLoaded, function (t) {
				return t === template;
			});

			if (!isLoaded) {
				require([template], function (t) {
					var $t = $(t);
					$('body').append($t);
					templateLoaded.push(template);
					defer.resolve(true);
				});
			}
			else {
				defer.resolve();
			}

			return defer.promise;
		}

		return {
			get: get,
			list: list
		};
	});
