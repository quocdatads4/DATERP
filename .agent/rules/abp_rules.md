# ABP Framework Rules & Best Practices

## 1. Module Development Standards
- **Folder Structure**: Follow the standard ABP module structure (Domain, Application, EntityFrameworkCore, HttpApi, Web).
- **Namespace Convention**: 
  - C#: CompanyName.ModuleName (e.g., DATERP.Examination)
  - JavaScript: companyName.moduleName (camelCase) (e.g., daterp.examination)

## 2. Dynamic JavaScript Proxies
- **Generation**: ABP automatically generates JS proxies for AppServices.
- **Root Namespace**: 
  - Providing the root namespace is DATERP (all caps), ABP's default CamelCasePropertyNamesContractResolver might convert it to dATERP (first char lower).
  - **Best Practice**: Always inspect /Abp/ServiceProxyScript to confirm the generated namespace.
- **Usage**:
  - Access services via the generated namespace: ar service = dATERP.examination.sample.sampleService;
  - Ensure the proxy script is loaded *before* your custom script.
  - If ReferenceError occurs, checking for casing issues (daterp vs dATERP) or ensure ConventionalControllers are configured.

## 3. Conventional Controllers
- **Configuration**:
  - Must explicitly register the module's Application assembly in the Web module's ConfigureServices method if not using AutoApiControllers on the module itself.
  `csharp
  context.Services.Configure<AbpAspNetCoreMvcOptions>(options =>
  {
      options.ConventionalControllers.Create(typeof(MyModuleApplicationModule).Assembly);
  });
  `
- **RemoteServiceName**:
  - Use [RemoteService(Name = ModuleName)] on AppServices to control the module grouping in API routes and JS proxies.

## 4. UI & Theming
- **Assets**: Store module-specific assets (JS, CSS) in 	hemes/ThemeName/wwwroot/ or wwwroot/ of the Web project if strictly project-specific.
- **Script Loading**: Use <abp-script> tag helper to ensure correct bundling and minification.

## 5. Automation & Testing
- **Server Readiness**: When running automation scripts that start the server, *always* implement a loop check for the server URL (e.g., http://localhost:5223) instead of a fixed sleep timer.
- **Browser Logs**: Capture browser console logs (logging.Type.BROWSER) in Selenium tests to debug frontend issues like missing variables or 404/500 errors.
