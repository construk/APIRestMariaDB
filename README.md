## API REST connected with MariaDB in ASP.Net Core using Pomelo

### Create the project
Create a new project Web Application ASP.Net Core.

----------------------------------------

### Necessary Library
Pomelo: [Pomelo.EntityFrameworkCore.MySql](https://github.com/PomeloFoundation/Pomelo.EntityFrameworkCore.MySql)

<sub>Tested in version 3.1.0 </sub>
 
*[Pomelo's license](https://github.com/construk/APIRestMariaDB/blob/master/LICENSEPOMELO.txt)*

----------------------------------------


#### appsettings.json

 ```{
  "connectionStrings": {
    "MySQL": "Server=YourServer.domain;Database=DBServer;User=YourDBUser;Password=YourDBPassword;"
  },
  ...
} 
```

* **Set connection Strings**
    * **Server:** Use your ip or your name domain where is installed the database.
    * **Database:** Select the database that you are going to connect.
    * **User:** Set the user from database that your going to log in.
    * **Password:** Set the password from database that your going to log in. 
    * **AllowedHosts:** You can restrict the connections to your API.

----------------------------------------

#### Startup.cs

```
//Adding DB Context
public void ConfigureServices(IServiceCollection services)
{
    ...
    services.AddDbContext<ApplicationDbContext>(options => options.UseMySql(Configuration.GetConnectionString("MySQL"), mysqlOptions => mysqlOptions.ServerVersion(new Version(10, 4, 11), ServerType.MariaDb)));
    ...
}
```

* **Set your version**

----------------------------------------

#### Configuration with Nginx and encryption with Let's Encrypt

```
server {
    server_name   YourDomain.domain;
    location / {
        proxy_pass         http://localhost:5000;
        proxy_http_version 1.1;
        proxy_set_header   Upgrade $http_upgrade;
        proxy_set_header   Connection keep-alive;
        proxy_set_header   Host $host;
        proxy_cache_bypass $http_upgrade;
        proxy_set_header   X-Forwarded-For $proxy_add_x_forwarded_for;
        proxy_set_header   X-Forwarded-Proto $scheme;
    }

    listen 443 ssl; # managed by Certbot
    ssl_certificate /etc/letsencrypt/live/YourDomain.domain/fullchain.pem; # managed by Certbot
    ssl_certificate_key /etc/letsencrypt/live/YourDomain.domain/privkey.pem; # managed by Certbot
    include /etc/letsencrypt/options-ssl-nginx.conf; # managed by Certbot
    ssl_dhparam /etc/letsencrypt/ssl-dhparams.pem; # managed by Certbot

}
server {
    if ($host = YourDomain.domain) {
        return 301 https://$host$request_uri;
    } # managed by Certbot


    listen        80;
    server_name   YourDomain.domain;
    return 404; # managed by Certbot

}
```

----------------------------------------

#### Creating class ApplicationDbContext

```
public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext (DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Test> Test { get; set; }

    public virtual DbSet<Test> TestList { get; set; }
        
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Test>(entity =>
        {
            entity.ToTable("test");
            entity.Property(e => e.Id)
            .HasColumnName("id")
            .HasColumnType("int(11)")
            .HasCharSet("utf8")
            .HasCollation("utf8_general_ci");
            entity.Property(e=>e.Nombre)
            .HasColumnName("nombre")
            .HasColumnType("varchar(20)")
            .HasDefaultValue("'NULL'")
            .HasCharSet("utf8")
            .HasCollation("utf8_general_ci");
        });
    }
}
```
* **Override the method OnModelCreating(ModelBuilder modelBuilder):** 
  * Set the target table.
  * Set the entity's fields.

----------------------------------------

#### Creating Model class

```
public class Test
{
    public Test(string nombre)
    {
        Nombre = nombre;
    }
    public int Id{ get; set; }
    public string Nombre{ get; set; }
}
```

----------------------------------------

#### Creating Controller class

* A class that inherit from Controller
* Define the path to your API with *Route*. In this example the API has the path *YourDomain.domain/api/test* .

```
[Route("api/test")]
[ApiController]
public class TestsController : Controller
{
    ...
}
```

* Define a field ApplicationDbContext.
 
```
private readonly ApplicationDbContext _context;
```

* A constructor that receive a ApplicationDbContext.
 
```
public TestsController(ApplicationDbContext context)
{
    _context = context;
}
```

* And finally define the statements using the ApplicationDbContext field that are going to execute your API Rest.

```
[HttpGet]
public async Task<ICollection<Test>> Get()
{
    return await _context.Test.ToListAsync();
}

[HttpGet("{id}")]
public async Task<ICollection<Test>> GetId(int id)
{
    return await _context.Test.Where(f=>f.Id==id).ToListAsync();
}

[HttpPost]
public async Task AddTest([FromForm] string nombre)
{
    Test test = new Test(nombre);
    this._context.Test.Add(test);
    await _context.SaveChangesAsync();
}
[HttpPut]
public async Task EditTest([FromForm]int id,[FromForm]string nombre)
{
    Test test = _context.Test.Where(t => t.Id == id).FirstOrDefault();
    test.Nombre = nombre;
    await _context.SaveChangesAsync();
}
[HttpDelete]
public async Task DeleteTest([FromForm]int id)
{
    _context.Test.Remove(_context.Test.Where(t => t.Id == id).FirstOrDefault());            
    await _context.SaveChangesAsync();
}
```

### Special thanks to contributor 
* [Guillermo Vera](https://github.githubassets.com/images/modules/logos_page/GitHub-Mark.png): Thanks for explaining this to me.