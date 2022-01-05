# JwtDemoProject
Token Based Authentication with Asp.net Core Web API 5.0 + JWT (JSON Web Token).

1. Select the “Asp.Net Core Web API” template
2. In Startup.cs file added in the Configure method :- app.UseAuthentication();
3. Create an interface :- IJwtAuth.cs and added this :- string Authentication(string username, string password);
4. "System.IdentityModel.Tokens.Jwt" and "Microsoft.AspNetCore.Authentication.JwtBearer"
5. Add a new class called "JwtAuth" and Implement the "IJwtAuth" interface. added Below lines
-------------------------------------------
````
        private readonly string username = "kirtesh";
        private readonly string password = "Demo1";
        private readonly string key;
        public Auth(string key)
        {
            this.key = key;
        }
        public string Authentication(string username, string password)
        {
            if (!(username.Equals(username) || password.Equals(password)))
            {
                return null;
            }

            // 1. Create Security Token Handler
            var tokenHandler = new JwtSecurityTokenHandler();

            // 2. Create Private Key to Encrypted
            var tokenKey = Encoding.ASCII.GetBytes(key);

            //3. Create JETdescriptor
            var tokenDescriptor = new SecurityTokenDescriptor()
            {
                Subject = new ClaimsIdentity(
                    new Claim[]
                    {
                        new Claim(ClaimTypes.Name, username)
                    }),
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(tokenKey), SecurityAlgorithms.HmacSha256Signature)
            };
            //4. Create Token
            var token = tokenHandler.CreateToken(tokenDescriptor);

            // 5. Return Token from method
            return tokenHandler.WriteToken(token);
        }
````
-------------------------------------------
6. Create UserCredential and Member
7. Create New Controller "MembersController" 
8. Added above Controller "[Authorize]" and "private readonly IJwtAuth jwtAuth;"
9. Create ctor
-------------------------------------------
		public MembersController(IJwtAuth jwtAuth)
        {
            this.jwtAuth = jwtAuth;
        }
-------------------------------------------
10. For Anonymous Added this lines
-------------------------------------------
		[AllowAnonymous]
        // POST api/<MembersController>
        [HttpPost("authentication")]
        public IActionResult Authentication([FromBody]UserCredential userCredential)
        {
            var token = jwtAuth.Authentication(userCredential.UserName, userCredential.Password);
            if (token == null)
                return Unauthorized();
            return Ok(token);
        }
------------------------------------------
11. Added this lines in Startup.cs
------------------------------------------
			var key = "This is my first Test Key";
            services.AddAuthentication(x =>
            {
                x.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                x.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            }).AddJwtBearer(x =>
            {
                x.RequireHttpsMetadata = false;
                x.SaveToken = true;
                x.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(key))
                };
            });

            services.AddSingleton<IJwtAuth>(new JwtAuth(key));
------------------------------------------
