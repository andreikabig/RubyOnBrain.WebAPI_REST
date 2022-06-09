using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using RubyOnBrain.API;
using RubyOnBrain.API.Services;
using RubyOnBrain.Data;
using RubyOnBrain.Domain;


// Cross-domain queries
var MyAllowSpecificOrigins = "_myAllowSpecificOrigins";

var builder = WebApplication.CreateBuilder(args);

// Cross-domain queries
builder.Services.AddCors(options =>
{
    options.AddPolicy(name: MyAllowSpecificOrigins,
                      policy =>
                      {
                          policy.WithOrigins("http://localhost:3000")
                                                  .AllowAnyHeader()
                                                  .AllowAnyMethod();
                      });
});

// Authorization and authentication services
builder.Services.AddAuthorization();
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options => // Adding options token configuration - JwtBarearOptions
    {
        options.TokenValidationParameters = new TokenValidationParameters   // token validation parameters
        {
            // specifies whether the publisher will be validated when validating the token
            ValidateIssuer = true,
            // a line representing the publisher
            ValidIssuer = AuthOptions.ISSUER,
            // whether the token user will be validated
            ValidateAudience = true,
            // setting the token consumer
            ValidAudience = AuthOptions.AUDIENCE,
            // will the time of existence be validated
            ValidateLifetime = true,
            // security key setting
            IssuerSigningKey = AuthOptions.GetSymmetricSecurityKey(),
            // security key validation
            ValidateIssuerSigningKey = true,
        };
    });

// Database connection string
string connection = builder.Configuration.GetConnectionString("DefaultConnection");

// Add a DataContext dependency 
builder.Services.AddDbContext<DataContext>(options => options.UseSqlServer(connection));

// Adding the ability to use controllers
builder.Services.AddControllers();

// Implementing the necessary dependencies
builder.Services.AddSingleton<RecommendationService>();
builder.Services.AddScoped<CourseService>();
builder.Services.AddScoped<UsersService>();
builder.Services.AddScoped<UserService>();
builder.Services.AddScoped<TopicService>();
builder.Services.AddScoped<EntryService>();



var app = builder.Build();

// Allow HTTPS if necessary
//app.UseHttpsRedirection();

// Middleware CORS
app.UseCors(MyAllowSpecificOrigins);

// Middleware authorization and authentication
app.UseAuthentication();
app.UseAuthorization();

// Middleware routing controllers
app.MapControllers();


app.Run();















//// ����������� ��������
//void HandleRouter(IApplicationBuilder appBuilder)
//{
//    appBuilder.Map("/method", HandleController);
//    appBuilder.Run(async (context) => await context.Response.WriteAsJsonAsync(new { error = "We can't find this one." }));
//}

//// ����������� �����������
//void HandleController(IApplicationBuilder appBuilder)
//{
//    appBuilder.Map("/courses", HandleMethodCourses);
//    appBuilder.Run(async (context) => await context.Response.WriteAsJsonAsync(new { error = "We can't find this object." }));
//}

//// ����������� ������
//void HandleMethodCourses(IApplicationBuilder appBuilder)
//{
//    appBuilder.Map("/show", MethodCoursesAll);
//    appBuilder.Run(async (context) => await context.Response.WriteAsJsonAsync(new { error = "We can't find this method." }));
//}

//// ����� ��������� ������ ���� ������
//void MethodCoursesAll(IApplicationBuilder appBuilder)
//{
//    // �����������: �������� �� ������ JSON ������ � �������� �� �� ������������
//    appBuilder.Use(async (context, next) => {
//        var response = context.Response;
//        var request = context.Request;

//        if (request.HasJsonContentType())
//        {
//            var jsonData = await request.ReadFromJsonAsync<RequestParams>();

//            if (jsonData != null)
//            {
//                // ���������� ����� ��������� ��������, � ��������� ������ ������

//                // ����������� ������ ���������� ������
//                string errorMessage = "";
//                foreach (var item in jsonData.requestParams)
//                {
//                    switch (item.key)
//                    {
//                        case "page":
//                            if (item.value != "all" && !Int32.TryParse(item.value, out int x))
//                                errorMessage += "Page value is not correct. ";
//                            break;
//                        case "page_size":
//                            if (!Int32.TryParse(item.value, out int y))
//                                errorMessage += "Page_size value is not correct. ";
//                            break;
//                        default:
//                            errorMessage += $"You sent an unknown parameter {item.key}";
//                            break;
//                    }
//                }

//                if (String.IsNullOrEmpty(errorMessage))
//                {
//                    await next.Invoke(context);
//                }
//                else
//                {
//                    Error error = new Error() { error_msg = errorMessage, request_params = jsonData };
//                    await response.WriteAsJsonAsync(error);
//                }
//            }
//        }
//        else
//        {
//            await response.WriteAsJsonAsync(new { error = "You'r request doesn't contain JSON data." });
//        }
//    });
    

//    // ������ ������ �������

//    appBuilder.Run(async (context) => {
//        var response = context.Response;
//        var request = context.Request;
//        var db = context.RequestServices.GetService<DataContext>();
//        var courseManager = context.RequestServices.GetService<CourseManager>();

//        if (request.HasJsonContentType())
//            Console.WriteLine($"\t-> JSON: {await request.ReadFromJsonAsync<RequestParams>()}");
//        else
//            Console.WriteLine("\t-> NOT JSON");

//        await response.WriteAsJsonAsync(new { info = "ok" });
//        //if (request.HasJsonContentType())
//        //{
//        //    var jsonData = await request.ReadFromJsonAsync<RequestParams>();
//        //    int page = 0;
//        //    int pageSize = 0;

//        //    foreach (var item in jsonData.requestParams)
//        //    {
//        //        switch (item.key)
//        //        {
//        //            case "page":
//        //                page = Convert.ToInt32(item.value);
//        //                break;
//        //            case "page_size":
//        //                pageSize = Convert.ToInt32(item.value);
//        //                break;
//        //            default:
//        //                await response.WriteAsJsonAsync(new { error = "Something went wrong." });
//        //                break;
//        //        }
//        //    }

//        //    if (page == 0 && pageSize == 0)
//        //    {
//        //        await response.WriteAsJsonAsync(db?.Courses.ToList());
//        //    }
//        //    else if (page > 0 && pageSize > 0)
//        //    {
//        //        // ��������� ���-�� ��������� ������
//        //        int? quantity = db?.Courses.Count();

//        //        // ���-�� ��������� �� ��������� ��������
//        //        int lastItemsCount = Convert.ToInt32(quantity % pageSize);

//        //        // ����� ��������� ��������
//        //        int lastPage = Convert.ToInt32(Math.Ceiling((decimal)quantity / (decimal)pageSize)); // ���� �� 1 ������

//        //        // ����������������, ����� ������ ������� �� pageSize ���������
//        //        int ps = pageSize;
//        //        // ���� ��������� ��������
//        //        if (page == lastPage)
//        //            ps = lastItemsCount;    // ������� = ���-�� ��������� �� ��������� ��������


//        //        if (page <= lastPage && page >= 1)
//        //        {
//        //            // ������� �������
//        //            var courses = db?.Courses.ToList().GetRange(pageSize * (page - 1), ps);
//        //            // ���������� ������� �������
//        //            await context.Response.WriteAsJsonAsync(courses);
//        //        }
//        //        else
//        //            await context.Response.WriteAsJsonAsync(new { error = "This page does not exist." });
//        //    }
//        //    else
//        //    {
//        //        await response.WriteAsJsonAsync(new { error = "Something went wrong." });
//        //    }
//        // }
//    }) ;
//}


class CourseManager
{
    DataContext db;
    public CourseManager(DataContext db)
    { 
        this.db = db;
    }

    public List<Course>? GetAllCourses()
    {
        return db?.Courses.ToList();
    }

    public List<Course>? GetCourses(int pageSize, int page)
    {
        // ��������� ���-�� ��������� ������
        int? quantity = db?.Courses.Count();

        // ���-�� ��������� �� ��������� ��������
        int lastItemsCount = Convert.ToInt32(quantity % pageSize);

        // ����� ��������� ��������
        int lastPage = Convert.ToInt32(Math.Ceiling((decimal)quantity / (decimal)pageSize)); // ���� �� 1 ������

        // ����������������, ����� ������ ������� �� pageSize ���������
        int ps = pageSize;
        // ���� ��������� ��������
        if (page == lastPage)
            ps = lastItemsCount;    // ������� = ���-�� ��������� �� ��������� ��������


        if (page <= lastPage && page >= 1)
        {
            // ������� �������
            var courses = db?.Courses.ToList().GetRange(pageSize * (page - 1), ps);
            // ���������� ������� �������
            return courses;
        }
        else
            return null;
    }


}