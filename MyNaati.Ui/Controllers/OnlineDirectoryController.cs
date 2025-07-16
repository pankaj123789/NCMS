//using System;
//using System.Collections.Generic;
//using System.Configuration;
//using System.Linq;
//using System.Web.Mvc;
//using System.Web.WebPages;
//using MyNaati.Contracts.BackOffice;
//using MyNaati.Ui.Attributes;
//using MyNaati.Ui.Common;
//using MyNaati.Ui.ViewModels.CredentialApplication;
//using MyNaati.Ui.ViewModels.PDSearch;

//namespace MyNaati.Ui.Controllers
//{
//    public class OnlineDirectoryController : BaseController
//    {
//        private readonly ICredentialApplicationService _credentialApplicationService;
//        private readonly List<ApplicationFormSectionModel> _sections;
//        private OnlineDirectorySearch _onlineDirectorySearch;

//        private const string IntroContent = @"<p><strong style=""text-decoration: underline;"">Using This Directory</strong><p>
//Practitioners listed in this directory have satisfied NAATI certification requirements.<br />
//Information in the directory is provided by participating translators and interpreters. Contact a practitioner directly for any questions about their specialisation, skills or qualifications. <br />
//NAATI strives for the highest professional standards, however we cannot vouch for the services of individual practitioners.";

//        public const int Translator = 1;
//        public const int Interpreter = 2;
//        public const int DeafInterpreter = 3;
//        public const int ConferenceInterpreter = 4;
//        public const int HealthSpecialistInterpreter = 5;
//        public const int LegalSpecialistInterpreter = 6;
//        public const int GeneralInterpreter = 7;

//        public OnlineDirectoryController(ICredentialApplicationService credentialApplicationService, OnlineDirectorySearch onlineDirectorySearch)
//        {
//            _credentialApplicationService = credentialApplicationService;
//            _onlineDirectorySearch = onlineDirectorySearch;

//            //_introLines.Clear();

//            _sections = new List<ApplicationFormSectionModel>
//            {
//                new ApplicationFormSectionModel
//                {
//                    Id = 1,
//                    Name = "Type",
//                    Questions = new List<ApplicationFormQuestionModel>
//                    {
//                        new ApplicationFormQuestionModel
//                        {
//                            Id = 1,
//                            Type = 1,//RadioOptions
//                            Description = "Do you need a translator or an interpreter?",
//                            Answers = new ApplicationFormAnswerModel[]
//                            {
//                                new ApplicationFormAnswerModel
//                                {
//                                    Id = Translator,
//                                    Name = "Translator"
//                                },
//                                new ApplicationFormAnswerModel
//                                {
//                                    Id = Interpreter,
//                                    Name = "Interpreter"
//                                },
//                                new ApplicationFormAnswerModel
//                                {
//                                    Id = DeafInterpreter,
//                                    Name = "Deaf Interpreter"
//                                }
//                            }
//                        },
//                        new ApplicationFormQuestionModel
//                        {
//                            Id = 2,
//                            Type = 1,//RadioOptions
//                            Description = "Are you looking for an interpreter who specialises in Conference Interpreting?",
//                            Answers = new ApplicationFormAnswerModel[]
//                            {
//                                new ApplicationFormAnswerModel
//                                {
//                                    Id = ConferenceInterpreter,
//                                    Name = "Yes"
//                                },
//                                new ApplicationFormAnswerModel
//                                {
//                                    Id = GeneralInterpreter,
//                                    Name = "No"
//                                }
//                            },
//                            Logics = new ApplicationFormQuestionLogicModel[]
//                            {
//                                new ApplicationFormQuestionLogicModel
//                                {
//                                    AnswerId = Interpreter
//                                }
//                            }
//                        }
//                    }
//                },
//                new ApplicationFormSectionModel
//                {
//                    Id = 2,
//                    Name = "Languages",
//                    Questions = new List<ApplicationFormQuestionModel>
//                    {
//                        new ApplicationFormQuestionModel
//                        {
//                            Id = 3,
//                            Type = 7,//Language selector
//                            Description = "Please select the language and direction for the translation.",
//                            Logics = new ApplicationFormQuestionLogicModel[]
//                            {
//                                new ApplicationFormQuestionLogicModel
//                                {
//                                    AnswerId = Translator
//                                }
//                            }
//                        },
//                        new ApplicationFormQuestionModel
//                        {
//                            Id = 4,
//                            Type = 7,//Language selector
//                            Description = "Please select the type of interpreter required?",
//                            Logics = new ApplicationFormQuestionLogicModel[]
//                            {
//                                new ApplicationFormQuestionLogicModel
//                                {
//                                    AnswerId = DeafInterpreter
//                                }
//                            }
//                        },
//                        new ApplicationFormQuestionModel
//                        {
//                            Id = 5,
//                            Type = 7,//Language selector
//                            Description = "Which language do you need?",
//                            Logics = new ApplicationFormQuestionLogicModel[]
//                            {
//                                new ApplicationFormQuestionLogicModel
//                                {
//                                    AnswerId = ConferenceInterpreter
//                                }
//                            }
//                        },
//                        new ApplicationFormQuestionModel
//                        {
//                            Id = 8,
//                            Type = 7,//Language selector
//                            Description = "Which language do you need?",
//                            Logics = new ApplicationFormQuestionLogicModel[]
//                            {
//                                new ApplicationFormQuestionLogicModel
//                                {
//                                    AnswerId = GeneralInterpreter
//                                }
//                            }
//                        }
//                    }
//                }
//            };
//        }

//        [HttpGet]
//        public ActionResult Index()
//        {
//            return View();
//        }

//        [HttpGet]
//        public ActionResult Settings()
//        {
//            foreach (var s in _sections)
//            {
//                s.Questions.Clear();
//            }
//            return Json(new
//            {
//                IntroContent,
//                Sections = _sections
//            }, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        public ActionResult NextQuestion(NextQuestionRequest request)
//        {
//            var currentSection = request.Form?.Sections?.FirstOrDefault(s => s.Id == request.SectionId);
//            var lastQuestion = currentSection?.Questions?.LastOrDefault();
//            var questions = _sections.FirstOrDefault(s => currentSection == null || s.Id == currentSection.Id)?.Questions;
//            var questionFound = lastQuestion == null ? true : false;
//            var nextQuestion = new ApplicationFormQuestionModel();

//            foreach (var q in questions)
//            {
//                if (questionFound)
//                {
//                    var show = true;

//                    if (q.Logics?.Any() ?? false)
//                    {
//                        var responses = request.Form?.Sections?.Where(s => s.Questions?.Any() ?? false).SelectMany(s => s.Questions).Select(x => x.Response).ToList();
//                        show = (responses?.Any() ?? false) && q.Logics.All(l => responses.Contains(l.AnswerId));
//                    }

//                    if (show)
//                    {
//                        nextQuestion = q;
//                        break;
//                    }
//                    else
//                    {
//                        continue;
//                    }
//                }

//                questionFound = q.Id == lastQuestion.Id;
//            }

//            return Json(nextQuestion);
//        }

//        [HttpPost]
//        public ActionResult Languages(SaveApplicationFormRequestModel request)
//        {
//            var lastQuestion = request?.Sections?.Where(s => s.Questions?.Any() ?? false).SelectMany(s => s.Questions)?.LastOrDefault();
//            var response = Convert.ToInt32(lastQuestion.Response);
//            var credentialTypes = new List<int>();
//            switch (response)
//            {
//                case Translator:
//                    credentialTypes = new List<int> { 1, 2, 3, 4, 28 };
//                    break;
//                case DeafInterpreter:
//                    credentialTypes = new List<int> { 12, 13 };
//                    break;
//                case ConferenceInterpreter:
//                    credentialTypes = new List<int> { 10, 11, 24 };
//                    break;
//                case HealthSpecialistInterpreter:
//                    credentialTypes = new List<int> { 8, 22 };
//                    break;
//                case LegalSpecialistInterpreter:
//                    credentialTypes = new List<int> { 9, 23 };
//                    break;
//                case GeneralInterpreter:
//                    credentialTypes = new List<int> { 5, 19, 25, 6, 20, 26, 7, 21, 27 };
//                    break;
//            }

//            var languages = _credentialApplicationService.GetLanguagesForCredentialTypes(credentialTypes);
//            var languagesModels = languages
//                .Results
//                .GroupBy(l => new { Language1Id = l.Language1Id, Language2Id = l.Language2Id, DisplayName = l.DisplayName })
//                .Select(s => new
//                {
//                    Id = $"{s.Key.Language1Id}|{s.Key.Language2Id}${String.Join(",", s.Select(k => k.Id.ToString()))}",
//                    s.Key.DisplayName
//                });

//            return Json(languagesModels, JsonRequestBehavior.AllowGet);
//        }

//        [HttpPost]
//        [MvcRecaptchaValidation(modelErrorKeys: nameof(PDSearchModel.ReCaptchaErrorMessage))]
//        public ActionResult NewSearch(PDSearchModel searchModel)
//        {
//            var model = _onlineDirectorySearch.GetNewSearchModel(true, searchModel.Skills);
//            if (searchModel != null)
//            {
//                if (searchModel.FirstLanguageId != 0 || searchModel.SecondLanguageId != 0)
//                {
//                    model.FirstLanguageId = searchModel.FirstLanguageId;
//                    model.SecondLanguageId = searchModel.SecondLanguageId;
//                    model.Skills = searchModel.Skills;
//                }
//            }

//            return View("~/Views/PDSearch/NewSearch.cshtml", model);
//        }

//        [HttpGet]
//        public ActionResult NewSearch()
//        {
//            return RedirectToAction("Index", "OnlineDirectory");
//        }
//    }

//    
//}

