using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ViewEngines;
using Microsoft.AspNetCore.Mvc.ViewFeatures;

namespace Infrastructure.AspComponents.Extensions;

/// <summary>
/// Методы-расширения для <see cref="Controller" />.
/// </summary>
public static class ControllerExtensions
{
    /// <summary>
    /// Получаем данные в виде кортежа из результата Представления <paramref name="objectViewResult"/>,
    /// который может быть или <see cref="ViewResult"/>, или <see cref="PartialViewResult"/>.
    /// </summary>
    private static (string ViewName, ViewDataDictionary ViewData, ITempDataDictionary TempData)
        GetViewResultData(this ControllerBase controller, object objectViewResult)
    {
        string? viewName = null;
        ViewDataDictionary? viewData = null;
        ITempDataDictionary? tempData = null;
        var viewNameDef = controller.ControllerContext.ActionDescriptor.ActionName;
        
        switch (objectViewResult)
        {
            case ViewResult viewResult:
                viewName = viewResult.ViewName ?? viewNameDef;
                viewData = viewResult.ViewData;
                tempData = viewResult.TempData;
                break;
            case PartialViewResult partialViewResult:
                viewName = partialViewResult.ViewName ?? viewNameDef;
                viewData = partialViewResult.ViewData;
                tempData = partialViewResult.TempData;
                break;
        }
        
        return (viewName!, viewData!, tempData!);
    }

    /// <summary>
    /// Преобразовать результат Представления в строку;
    ///  в качестве параметров - вытащенные из результата Представления данные.
    /// </summary>
    private static string ConvertViewToString(this ControllerBase controller, 
        string viewName, ViewDataDictionary viewData, ITempDataDictionary tempData, 
        ICompositeViewEngine viewEngine)
    {
        using var writer = new StringWriter();
        
        var view = viewEngine.FindView(controller.ControllerContext, viewName, false).View;
        if (view != null)
        {
            var viewContext = new ViewContext(
                controller.ControllerContext, 
                view,
                viewData,
                tempData,
                writer,
                new HtmlHelperOptions()
            );
            view.RenderAsync(viewContext).Wait();
        }

        return writer.GetStringBuilder().ToString();
    }

    /// <summary>
    /// Преобразовать результат Представления в строку (асинхронный вариант);
    ///  в качестве параметров - вытащенные из результата Представления данные.
    /// </summary>
    private static async Task<string> ConvertViewToStringAsync(this ControllerBase controller, 
        string viewName, ViewDataDictionary viewData, ITempDataDictionary tempData, 
        ICompositeViewEngine viewEngine)
    {
        await using var writer = new StringWriter();
        
        var view = viewEngine.FindView(controller.ControllerContext, viewName, false).View;
        if (view != null)
        {
            var viewContext = new ViewContext(
                controller.ControllerContext, 
                view,
                viewData,
                tempData,
                writer,
                new HtmlHelperOptions()
            );
            await view.RenderAsync(viewContext);
        }

        return writer.GetStringBuilder().ToString();
    }
    
    /// <summary>
    /// Преобразовать результат Представления в строку.
    /// </summary>
    /// <param name="controller">Контроллер.</param>
    /// <param name="partialViewResult">Результат Представления.</param>
    /// <param name="viewEngine">Механизм рендеринга Представления.</param>
    public static string ConvertViewToString(this Controller controller, 
        ViewResult partialViewResult, ICompositeViewEngine viewEngine)
    {
        // Получаем данные из partialViewResult
        var viewResultData = controller.GetViewResultData(partialViewResult);

        return controller.ConvertViewToString(
            viewResultData.ViewName,
            viewResultData.ViewData,
            viewResultData.TempData,
            viewEngine);
    }
    
    /// <summary>
    /// Преобразовать результат частичного Представления в строку.
    /// </summary>
    /// <param name="controller">Контроллер.</param>
    /// <param name="partialViewResult">Результат Представления.</param>
    /// <param name="viewEngine">Механизм рендеринга Представления.</param>
    public static string ConvertViewToString(this Controller controller, 
        PartialViewResult partialViewResult, ICompositeViewEngine viewEngine)
    {
        // Получаем данные из partialViewResult
        var viewResultData = controller.GetViewResultData(partialViewResult);

        return controller.ConvertViewToString(
            viewResultData.ViewName,
            viewResultData.ViewData,
            viewResultData.TempData,
            viewEngine);
    }
    
    /// <inheritdoc cref="ConvertViewToString(Controller, ViewResult, ICompositeViewEngine)"/>
    /// <remarks>Асинхронный вариант.</remarks>
    // TODO: Протестировать!
    public static async Task<string> ConvertViewToStringAsync(this Controller controller, 
        ViewResult partialViewResult, ICompositeViewEngine viewEngine)
    {
        // Получаем данные из partialViewResult
        var viewResultData = controller.GetViewResultData(partialViewResult);

        return await controller.ConvertViewToStringAsync(
            viewResultData.ViewName,
            viewResultData.ViewData,
            viewResultData.TempData,
            viewEngine);
    }
    
    /// <inheritdoc cref="ConvertViewToString(Controller, PartialViewResult, ICompositeViewEngine)"/>
    /// <remarks>Асинхронный вариант.</remarks>
    // TODO: Протестировать!
    public static async Task<string> ConvertViewToStringAsync(this Controller controller, 
        PartialViewResult partialViewResult, ICompositeViewEngine viewEngine)
    {
        // Получаем данные из partialViewResult
        var viewResultData = controller.GetViewResultData(partialViewResult);

        return await controller.ConvertViewToStringAsync(
            viewResultData.ViewName,
            viewResultData.ViewData,
            viewResultData.TempData,
            viewEngine);
    }

    /// <summary>
    /// Получить коллекцию ошибок ModelState.
    /// </summary>
    public static IEnumerable<ModelError> GetModelStateErrors(this ControllerBase controller)
    {
        return controller.ModelState.Values.SelectMany(v => v.Errors);
    }
}