
using TextToSpeech.Services;

namespace TextToSpeech
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            {
                builder.Services.AddCors(options =>
                {
                    options.AddPolicy("AllowLocalhost", policy =>
                    {
                        policy.WithOrigins("https://insooeric.github.io")
                              .AllowAnyHeader()
                              .AllowAnyMethod()
                              .SetIsOriginAllowedToAllowWildcardSubdomains()
                              .AllowCredentials();
                    });
                });
            }
            builder.Services.AddSingleton<RealtimeDatabaseService>();
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();

            app.UseCors("AllowLocalhost");
            app.UseStaticFiles();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
