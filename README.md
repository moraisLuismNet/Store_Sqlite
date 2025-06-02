## Store_Sqlite

**Store_Sqlite** is an ASP.NET Core web API for a store, managing categories, products, and users. It includes controllers for CRUD operations and features such as pagination, filtering, image file management, and JWT authentication/authorization. It implements services for action logging, file management, password encryption and hashing, and a scheduled task. Entity Framework Core is configured for interaction with a Sqlite database, with DTOs for data transfer and validators for data input. Finally, development and production configurations, a Dockerfile, and a Docker-Compose for container deployment are provided.

![Store_Sqlite](img/UML.png)

Store_Sqlite/        
├───Classes/  
│   └───ResultHash.cs  
├───Controllers/  
│   ├───ActionsController.cs  
│   ├───CategoriesController.cs  
│   ├───ProductsController.cs  
│   └───UsersController.cs  
├───DTOs/  
│   ├───CategoryDTO.cs  
│   ├───CategoryInsertDTO.cs  
│   ├───CategoryItemDTO.cs  
│   ├───CategoryProductDTO.cs  
│   ├───CategoryUpdateDTO.cs  
│   ├───LoginResponseDTO.cs  
│   ├───ProductDTO.cs  
│   ├───ProductFilterDTO.cs  
│   ├───ProductInsertDTO.cs  
│   ├───ProductSaleDTO.cs  
│   ├───ProductUpdateDTO.cs  
│   ├───UserChangePasswordDTO.cs  
│   └───UserDTO.cs  
├───Filters/  
│   └───ExceptionFilter.cs  
├───Middlewares/  
│   └───RegisterAndControlMiddleware.cs  
├───Models/  
│   ├───Action.cs  
│   ├───Category.cs  
│   ├───Product.cs  
│   ├───StoreContext.cs  
│   └───User.cs  
├───Services/  
│   ├───ActionsService.cs  
│   ├───FileManagerService.cs  
│   ├───HashService.cs  
│   ├───IFileManagerService.cs  
│   ├───ScheduledTaskService.cs  
│   └───TokenService.cs  
├───Validators/  
│   ├───GroupFileType.cs  
│   ├───ValidationFileType.cs  
│   └───WeightFileValidation.cs  
├───appsettings.json  
└───Program.cs    
└───store.db 


![Store_Sqlite](img/1.png)
![Store_Sqlite](img/2.png)


## Program
``` 
var connectionString = builder.Configuration.GetConnectionString("Connection");

builder.Services.AddDbContext<StoreContext>(options =>
    options.UseSqlite(connectionString);
);
``` 

## appsetting.Development.json
``` 
{
  "ConnectionStrings": {
    "Connection": "Data Source=store.db"
  }
}
``` 

![Store_Sqlite](img/DB.png)

[DeepWiki moraisLuismNet/Store_Sqlite](https://deepwiki.com/moraisLuismNet/Store_Sqlite)

