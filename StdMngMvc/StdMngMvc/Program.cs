using System;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StdMngMvc.Data;

//设置策略名称
var MyAllowSpecificOrigins = "any";

var builder = WebApplication.CreateBuilder(args);

// 添加数据库上下文
builder.Services.AddDbContext<SchoolContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// 添加MVC服务
builder.Services.AddRazorPages();


//在 var app = builder.Build() 语句之前之注册跨域服务
builder.Services.AddCors(options =>
{
    options.AddPolicy(name:MyAllowSpecificOrigins, builder =>
    {
        builder.AllowAnyOrigin()
        .AllowAnyMethod()
        .AllowAnyHeader();
        //.AllowCredentials(); 指定cookie与策略“any”冲突

    });
});

var app = builder.Build();

// 配置请求管道
if (app.Environment.IsDevelopment())
{
    app.UseDeveloperExceptionPage();
}
else
{
    app.UseExceptionHandler("./Shared/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

//在MapControllerRoute之前弃用跨域中间件，最好一开始就启用 传入策略，参数为空表示使用默认策略
app.UseCors("any");

app.UseAuthorization();

app.MapRazorPages();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();