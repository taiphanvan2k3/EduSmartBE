using CourseManagementService.Common;
using CourseManagementService.Common.Helpers;
using CourseManagementService.Common.Schemas;
using CourseManagementService.Services.AppState.Schemas;
using CourseManagementService.Services.BookmarkManagement;
using CourseManagementService.Services.Cache;
using CourseManagementService.Services.CategoryManagement.Schemas;
using CourseManagementService.Services.ChapterManagement;
using CourseManagementService.Services.CourseManagement.Public.Schemas;
using CourseManagementService.Services.CourseManagement.Student;
using CourseManagementService.Services.CourseManagement.Teacher.Schemas;
using CourseManagementService.Services.Grpc.PaymentService;
using CourseManagementService.Services.Grpc.PaymentService.Schemas;
using CourseManagementService.Services.Grpc.UserService;
using CourseManagementService.Services.LessonManagement.LessonBase;
using CourseManagementService.Services.LessonManagement.LessonBase.Schemas;
using CourseManagementService.Services.Medias;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;

namespace CourseManagementService.Services.CourseManagement.Public
{
    public interface IPublicCourseDetailService
    {
        /// <summary>
        /// Get course detail by course ID
        /// <para>Created at: 2024/11/06</para>
        /// <para>Created by: TaiPV</para>
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public Task<CourseDetail> GetCourseDetail(Guid courseId);

        /// <summary>
        /// Check if user can access course material
        /// </summary>
        /// <param name="courseId">Id of course</param>
        /// <returns></returns>
        public Task<bool> CanAccessCourseMaterial(Guid courseId);

        /// <summary>
        /// Check a user's registration status for a course
        /// </summary>
        /// <param name="courseId">Id of course</param>
        /// <returns></returns>
        public Task<ResponseInfo> GetRegistrationStatus(Guid courseId);

        /// <summary>
        /// Generate QR code for payment
        /// <para>Created at: 2024/10/20</para>
        /// <para>Created by: TaiPV</para> 
        /// </summary>
        /// <param name="courseId"></param>
        /// <returns></returns>
        public Task<ResponseInfo> GetCoursePaymentInfo(Guid courseId);
    }

    public class PublicCourseDetailService(IServiceProvider serviceProvider,
        ILogger<ListOfPublicCourseService> logger)
        : BaseService(serviceProvider, logger), IPublicCourseDetailService
    {
        private readonly IGrpcUserService _grpcUserService = serviceProvider.GetService<IGrpcUserService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IGrpcUserService"));
        private readonly IGrpcPaymentService _grpcPaymentService = serviceProvider.GetService<IGrpcPaymentService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IGrpcPaymentService"));
        private readonly ICacheService _cacheService = serviceProvider.GetRequiredService<ICacheService>()
            ?? throw new InvalidOperationException(ServiceInjectionError("ICacheService"));
        private readonly ILessonBaseDetailService _lessonBaseDetailService = serviceProvider.GetService<ILessonBaseDetailService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("ILessonBaseDetailService"));
        private readonly IListOfChaptersService _chapterService = serviceProvider.GetService<IListOfChaptersService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IListOfChaptersService"));
        private readonly IVideoService _videoService = serviceProvider.GetService<IVideoService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IVideoService"));
        private readonly IStudentCourseDetailService _studentCourseDetailService = serviceProvider.GetService<IStudentCourseDetailService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IStudentCourseDetailService"));
        private readonly IBookmarkService _bookmarkService = serviceProvider.GetService<IBookmarkService>()
            ?? throw new ArgumentNullException(ServiceInjectionError("IBookmarkService"));

        public Task<bool> CanAccessCourseMaterial(Guid courseId)
        {
            try
            {
                var currentUser = GetCurrentUser();
                return _lessonBaseDetailService.CanAccessCourseMaterial(courseId, currentUser.UserId);
            }
            catch (Exception e)
            {
                LogError(e, GetActualAsyncMethodName());
                throw;
            }
        }

        public async Task<CourseDetail> GetCourseDetail(Guid courseId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                UserInfoState currentUser = GetCurrentUser();
                string cacheKey;
                if (currentUser == null || !await _lessonBaseDetailService.CanAccessCourseMaterial(courseId, currentUser.UserId))
                {
                    cacheKey = CacheManager.CourseDetail.Key(courseId, 0);
                }
                else
                {
                    cacheKey = CacheManager.CourseDetail.Key(courseId, currentUser.UserId);
                }

                var courseDetail = _cacheService.GetData<CourseDetail>(cacheKey);
                if (courseDetail != null)
                {
                    if (courseDetail.Course.IsRegistered)
                    {
                        courseDetail.UnlockedLessons = await _lessonBaseDetailService.GetUnlockedLessons(courseId, currentUser.UserId);
                        courseDetail.BookmarkedLessonIds = await _bookmarkService.GetBookmarkedLessonIds(courseId);
                    }
                    return courseDetail;
                }

                courseDetail = await _context.Courses
                    .Where(x => x.Id == courseId)
                    .Select(x => new CourseDetail()
                    {
                        Teacher = new TeacherDetail()
                        {
                            Id = x.TeacherId
                        },
                        Course = new CourseDto()
                        {
                            Id = x.Id,
                            Name = x.Name,
                            Description = x.Description,
                            CoreValues = x.CoreValues,
                            Prerequisites = x.Prerequisites,
                            Price = x.Price,
                            CurrencyCode = x.Currency.Code,
                            Type = new LookupDto()
                            {
                                Id = EnumHelper.ConvertEnumToInt(x.Type).ToString(),
                                Name = x.Type.ToString()
                            },
                            Category = new CategoryDto()
                            {
                                Id = x.Category.Id,
                                Name = x.Category.Name,
                                WebIconInfo = new IconInfoDto()
                                {
                                    Icon = x.Category.WebIconInfo.Icon,
                                    Color = x.Category.WebIconInfo.Color
                                },
                                MobileIconInfo = new IconInfoDto()
                                {
                                    Icon = x.Category.MobileIconInfo.Icon,
                                    Color = x.Category.MobileIconInfo.Color
                                }
                            },
                            Tags = x.Tags.Select(x => new LookupDto()
                            {
                                Id = x.Tag.Id.ToString(),
                                Name = x.Tag.Name
                            })
                            .ToList(),
                            ThumbnailURL = x.ThumbnailURL,
                            PreviewVideoURL = x.PreviewVideoURL,
                            TotalStudents = x.Enrollments.Count,
                            TotalLessons = x.Chapters.SelectMany(x => x.Lessons).Count(),
                            TotalSecondsByChapter = x.Chapters.Select(c => c.Lessons.Sum(l => l.DurationInSeconds)).ToList(),
                            IsRegistered = currentUser != null && (
                                x.Enrollments.Any(e => e.StudentId == currentUser.UserId)
                                || x.TeacherId == currentUser.UserId
                            ),
                            IsPublished = x.IsPublished
                        }
                    })
                    .FirstOrDefaultAsync();

                if (courseDetail == null)
                {
                    return null;
                }

                if (courseDetail.Course.PreviewVideoURL != null)
                {
                    courseDetail.Course.PreviewVideoURL = _videoService.GetVideoURLWithSAS(courseDetail.Course.PreviewVideoURL);
                }

                await FillTeacherInfo(courseDetail);

                var isTeacher = currentUser != null && courseDetail.Teacher.Id == currentUser.UserId;
                courseDetail.Chapters = await _chapterService.GetListOfChaptersByCourseId(courseId, isTeacher);

                if (courseDetail.Course.IsRegistered)
                {
                    courseDetail.UnlockedLessons = await _lessonBaseDetailService.GetUnlockedLessons(courseId, currentUser.UserId);
                    courseDetail.BookmarkedLessonIds = await _bookmarkService.GetBookmarkedLessonIds(courseId);
                }

                if (courseDetail.Chapters.Count > 0)
                {
                    var firstLesson = courseDetail.Chapters[0].Lessons.FirstOrDefault();
                    courseDetail.Course.FirstLesson = firstLesson != null ? new LessonInfoBase()
                    {
                        Id = firstLesson.Id,
                        LessonType = firstLesson.LessonType
                    } : null;
                }

                if (courseDetail.UnlockedLessons.Count == 0 && courseDetail.Course.FirstLesson != null)
                {
                    courseDetail.UnlockedLessons.Add(new LessonTrackingDetail()
                    {
                        LessonId = courseDetail.Course.FirstLesson.Id,
                        LessonOrder = 1,
                        ChapterOrder = 1,
                        TimeSpent = 0,
                        IsCompleted = false
                    });
                }

                _cacheService.SetData(cacheKey, courseDetail, DateTimeOffset.Now.AddMinutes(CacheManager.CourseDetail.ExpireTimeInMinutes));

                LogInfo("End", method);
                return courseDetail;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        public async Task<ResponseInfo> GetRegistrationStatus(Guid courseId)
        {
            var methodName = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", methodName);
                var isCourseExist = await _context.Courses.AnyAsync(x => x.Id == courseId);
                if (!isCourseExist)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Course not found");
                }

                var responseInfo = new ResponseInfo();
                bool isRegistered = await CanAccessCourseMaterial(courseId);
                responseInfo.Data.Add("isRegistered", isRegistered);

                LogInfo("End", methodName);
                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, methodName);
                throw;
            }
        }

        public async Task<ResponseInfo> GetCoursePaymentInfo(Guid courseId)
        {
            var method = GetActualAsyncMethodName();
            try
            {
                LogInfo("Start", method);
                var responseInfo = new ResponseInfo();
                var currentUser = GetCurrentUser();

                if (await _studentCourseDetailService.IsCourseEnrolled(courseId))
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "You have already enrolled this course");
                }

                var course = await _context.Courses
                    .Where(x => x.Id == courseId)
                    .Select(x => new
                    {
                        x.Id,
                        x.Price,
                        CurrencyCode = x.Currency.Code,
                        x.Name,
                        x.TeacherId
                    })
                    .FirstOrDefaultAsync();

                if (course == null)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status404NotFound, "Course not found");
                }

                if (course.TeacherId == currentUser.UserId)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status400BadRequest, "You cannot enroll your own course");
                }

                var relatedInfo = JsonConvert.SerializeObject(new
                {
                    courseId = course.Id,
                    teacherId = course.TeacherId
                });

                var transactionCode = $"SEP{Utils.GenerateRandomString(9)}";
                var currentUserInfo = await _grpcUserService.GetUserInfoWithRole(currentUser.UserId);

                // Get bank account and create transaction through gRPC
                var getBankAccountTask = _grpcPaymentService.GetAdminBankAccountAsync();
                var createTransactionTask = _grpcPaymentService.CreateCoursePaymentTransactionAsync(new PaymentTransactionData
                {
                    Amount = (double)course.Price,
                    RelatedInfo = relatedInfo,
                    CreatedBy = new UserDetail()
                    {
                        Id = currentUser.UserId,
                        Username = currentUser.UserName,
                        FullName = currentUserInfo.FullName,
                        Email = currentUserInfo.Email,
                        AvatarURL = currentUserInfo.AvatarURL
                    },
                    ReceiverId = course.TeacherId,
                    TransactionCode = transactionCode,
                    CurrencyCode = course.CurrencyCode
                });

                await Task.WhenAll(getBankAccountTask, createTransactionTask);

                // Handle response from gRPC
                ResponseInfo bankAccountResponse = getBankAccountTask.Result;
                ResponseInfo transactionResponse = createTransactionTask.Result;

                if (!bankAccountResponse.IsSuccess || !transactionResponse.IsSuccess)
                {
                    return CreateEarlyResponseInfo(StatusCodes.Status500InternalServerError,
                        "Failed to get bank account or create transaction");
                }

                var adminAccount = bankAccountResponse.Data["adminAccount"] as BankAccountDto;
                var transactionId = transactionResponse.Data["transactionId"] as string;
                var usdToVndRate = transactionResponse.Data["exchangeRate"] as double? ?? 25397;

                var coursePaymentInfo = GetCoursePaymentInfo(adminAccount, course, transactionId, transactionCode, usdToVndRate);
                responseInfo.Data.Add("coursePaymentInfo", coursePaymentInfo);

                return responseInfo;
            }
            catch (Exception e)
            {
                LogError(e, method);
                throw;
            }
        }

        private async Task FillTeacherInfo<T>(T course) where T : ICourseWithTeacher
        {
            var teachers = await _grpcUserService.GetListOfTeachers([course.Teacher.Id]);
            var teacher = teachers.Users.FirstOrDefault();

            if (teacher != null)
            {
                course.Teacher.FullName = teacher.FullName;
                course.Teacher.AvatarURL = teacher.AvatarURL;
                course.Teacher.Email = teacher.Email;
            }
        }

        private static CoursePaymentInfo GetCoursePaymentInfo(BankAccountDto adminAccount, dynamic courseInfo,
            string transactionId, string transactionCode, double usdToVndRate)
        {
            var qrData = new CoursePaymentQRData()
            {
                Amount = Utils.ExchangeCurrency(courseInfo.Price, courseInfo.CurrencyCode, "VND", (decimal)usdToVndRate),
                AdminAccount = adminAccount,
                TransactionOrder = transactionCode
            };

            var qrURL = Utils.GenerateQRCodeForCoursePayment(courseInfo.Id, qrData);
            return new CoursePaymentInfo()
            {
                TransactionId = transactionId,
                CourseName = courseInfo.Name,
                PriceAtVnd = qrData.Amount,
                PriceAtUsd = Utils.ExchangeCurrency(qrData.Amount, "VND", "USD", (decimal)usdToVndRate),
                AccountName = adminAccount.AccountName,
                AccountNumber = adminAccount.AccountNumber,
                BankName = adminAccount.BankName,
                QRCode = qrURL
            };
        }
    }
}