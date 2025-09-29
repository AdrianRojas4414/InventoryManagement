˛
sC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\WeatherForecast.cs
	namespace 	
InventoryManagement
 
; 
public 
class 
WeatherForecast 
{ 
public 

DateOnly 
Date 
{ 
get 
; 
set  #
;# $
}% &
public 

int 
TemperatureC 
{ 
get !
;! "
set# &
;& '
}( )
public		 

int		 
TemperatureF		 
=>		 
$num		 !
+		" #
(		$ %
int		% (
)		( )
(		) *
TemperatureC		* 6
/		7 8
$num		9 ?
)		? @
;		@ A
public 

string 
? 
Summary 
{ 
get  
;  !
set" %
;% &
}' (
} Á
kC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Program.cs
var 
builder 
= 
WebApplication 
. 
CreateBuilder *
(* +
args+ /
)/ 0
;0 1
Env		 
.		 
Load		 
(		 	
Path			 
.		 
Combine		 
(		 
	Directory		 
.		  
GetCurrentDirectory		  3
(		3 4
)		4 5
,		5 6
$str		7 ;
,		; <
$str		= C
)		C D
)		D E
;		E F
var 
dbHost 

= 
$str 
; 
var 
dbName 

= 
$str $
;$ %
var 
dbUser 

= 
Env 
. 
	GetString 
( 
$str $
)$ %
;% &
var 
dbPass 

= 
Env 
. 
	GetString 
( 
$str (
)( )
;) *
var 
connectionString 
= 
$" 
$str  
{  !
dbHost! '
}' (
$str( 2
{2 3
dbName3 9
}9 :
$str: @
{@ A
dbUserA G
}G H
$strH R
{R S
dbPassS Y
}Y Z
$strZ [
"[ \
;\ ]
builder 
. 
Services 
. 
AddDbContext 
< 
InventoryDbContext 0
>0 1
(1 2
options2 9
=>: <
options 
. 
UseMySql 
( 
connectionString %
,% &
ServerVersion' 4
.4 5

AutoDetect5 ?
(? @
connectionString@ P
)P Q
)Q R
) 
; 
builder 
. 
Services 
. 
AddControllers 
(  
)  !
;! "
builder 
. 
Services 
. #
AddEndpointsApiExplorer (
(( )
)) *
;* +
builder 
. 
Services 
. 
AddSwaggerGen 
( 
)  
;  !
var 
app 
= 	
builder
 
. 
Build 
( 
) 
; 
if!! 
(!! 
app!! 
.!! 
Environment!! 
.!! 
IsDevelopment!! !
(!!! "
)!!" #
)!!# $
{"" 
app## 
.## 

UseSwagger## 
(## 
)## 
;## 
app$$ 
.$$ 
UseSwaggerUI$$ 
($$ 
)$$ 
;$$ 
}%% 
app'' 
.'' 
UseHttpsRedirection'' 
('' 
)'' 
;'' 
app)) 
.)) 
UseAuthorization)) 
()) 
))) 
;)) 
app++ 
.++ 
MapControllers++ 
(++ 
)++ 
;++ 
app-- 
.-- 
Run-- 
(-- 
)-- 	
;--	 
∫
ëC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Infrastructure\Repositories\ProductRepository.cs
	namespace 	
InventoryManagement
 
. 
Infrastructure ,
., -
Repositories- 9
;9 :
public 
class 
ProductRepository 
:  
IProductRepository! 3
{		 
} ∂
ëC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Infrastructure\Persistence\InventoryDbContext.cs
	namespace 	
InventoryManagement
 
. 
Infrastructure ,
., -
Persistence- 8
;8 9
public 
class 
InventoryDbContext 
:  !
	DbContext" +
{ 
public 

InventoryDbContext 
( 
DbContextOptions .
<. /
InventoryDbContext/ A
>A B
optionsC J
)J K
:L M
baseN R
(R S
optionsS Z
)Z [
{		 
}

 
public 

DbSet 
< 
Product 
> 
Products "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

DbSet 
< 
Supplier 
> 
	Suppliers $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

DbSet 
< 
Category 
> 

Categories %
{& '
get( +
;+ ,
set- 0
;0 1
}2 3
public 

DbSet 
< 
User 
> 
Users 
{ 
get "
;" #
set$ '
;' (
}) *
public 

DbSet 
< 
Purchase 
> 
	Purchases $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

DbSet 
< 
PurchaseDetail 
>  
PurchaseDetails! 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
public 

DbSet 
< 
SupplierProduct  
>  !
SupplierProducts" 2
{3 4
get5 8
;8 9
set: =
;= >
}? @
public 

DbSet 
< 
ProductPriceHistory $
>$ %!
ProductPriceHistories& ;
{< =
get> A
;A B
setC F
;F G
}H I
	protected 
override 
void 
OnModelCreating +
(+ ,
ModelBuilder, 8
modelBuilder9 E
)E F
{ 
modelBuilder 
. 
Entity 
< 
PurchaseDetail *
>* +
(+ ,
), -
. 
HasKey 
( 
pd 
=> 
new 
{ 
pd  "
." #

PurchaseId# -
,- .
pd/ 1
.1 2
	ProductId2 ;
}< =
)= >
;> ?
modelBuilder 
. 
Entity 
< 
SupplierProduct +
>+ ,
(, -
)- .
. 
HasKey 
( 
sp 
=> 
new 
{ 
sp  "
." #

SupplierId# -
,- .
sp/ 1
.1 2
	ProductId2 ;
}< =
)= >
;> ?
modelBuilder 
. 
Entity 
< 
ProductPriceHistory /
>/ 0
(0 1
)1 2
. 
HasKey 
( 
ph 
=> 
new 
{ 
ph  "
." #
	ProductId# ,
,, -
ph. 0
.0 1

SupplierId1 ;
,; <
ph= ?
.? @

PurchaseId@ J
}K L
)L M
;M N
} 
}   é
yC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Infrastructure\Class1.cs
	namespace 	
Infrastructure
 
; 
public 
class 
Class1 
{ 
} Œ
xC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\User.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
[ 
Table 
( 
$str 
) 
] 
public 
class 
User 
: 
AuditableEntity #
{ 
[		 
Key		 
]		 	
public

 

short

 
Id

 
{

 
get

 
;

 
set

 
;

 
}

  !
[ 
Required 
] 
[ 
	MaxLength 
( 
$num 
) 
] 
[ 
Column 
( 
$str 
) 
] 
public 

string 
Username 
{ 
get  
;  !
set" %
;% &
}' (
=) *
null+ /
!/ 0
;0 1
[ 
Required 
] 
[ 
	MaxLength 
( 
$num 
) 
] 
[ 
Column 
( 
$str 
) 
] 
public 

string 
PasswordHash 
{  
get! $
;$ %
set& )
;) *
}+ ,
=- .
null/ 3
!3 4
;4 5
[ 
Required 
] 
[ 
	MaxLength 
( 
$num 
) 
] 
[ 
Column 
( 
$str 
) 
] 
public 

string 
	FirstName 
{ 
get !
;! "
set# &
;& '
}( )
=* +
null, 0
!0 1
;1 2
[ 
Required 
] 
[ 
	MaxLength 
( 
$num 
) 
] 
[ 
Column 
( 
$str 
) 
] 
public 

string 
LastName 
{ 
get  
;  !
set" %
;% &
}' (
=) *
null+ /
!/ 0
;0 1
[   
	MaxLength   
(   
$num   
)   
]   
[!! 
Column!! 
(!! 
$str!! 
)!! 
]!!  
public"" 

string"" 
?"" 
SecondLastName"" !
{""" #
get""$ '
;""' (
set"") ,
;"", -
}"". /
[$$ 
Required$$ 
]$$ 
[%% 
	MaxLength%% 
(%% 
$num%% 
)%% 
]%% 
[&& 
Column&& 
(&& 
$str&& 
)&& 
]&& 
public'' 

string'' 
Role'' 
{'' 
get'' 
;'' 
set'' !
;''! "
}''# $
=''% &
null''' +
!''+ ,
;'', -
}(( æ
ÉC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\SupplierProduct.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
[ 
Table 
( 
$str 
) 
] 
public 
class 
SupplierProduct 
: 
AuditableEntity .
{ 
public 

short 

SupplierId 
{ 
get !
;! "
set# &
;& '
}( )
public		 

short		 
	ProductId		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
[ 
Column 
( 
TypeName 
= 
$str '
)' (
]( )
public 

decimal 
ProductCost 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

virtual 
Supplier 
Supplier $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

virtual 
Product 
Product "
{# $
get% (
;( )
set* -
;- .
}/ 0
} ª
|C:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\Supplier.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
[ 
Table 
( 
$str 
) 
] 
public 
class 
Supplier 
: 
AuditableEntity '
{ 
[		 
Key		 
]		 	
public

 

short

 
Id

 
{

 
get

 
;

 
set

 
;

 
}

  !
[ 
Required 
] 
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
? 
Nit 
{ 
get 
; 
set !
;! "
}# $
public 

string 
? 
Address 
{ 
get  
;  !
set" %
;% &
}' (
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
? 
Phone 
{ 
get 
; 
set  #
;# $
}% &
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
? 
Email 
{ 
get 
; 
set  #
;# $
}% &
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
? 
ContactName 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

virtual 
ICollection 
< 
SupplierProduct .
>. /
SupplierProducts0 @
{A B
getC F
;F G
setH K
;K L
}M N
}   ß
ÇC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\PurchaseDetail.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
[ 
Table 
( 
$str 
) 
] 
public 
class 
PurchaseDetail 
{ 
public 

int 

PurchaseId 
{ 
get 
;  
set! $
;$ %
}& '
public		 

short		 
	ProductId		 
{		 
get		  
;		  !
set		" %
;		% &
}		' (
public 

short 
Quantity 
{ 
get 
;  
set! $
;$ %
}& '
[ 
Column 
( 
TypeName 
= 
$str '
)' (
]( )
public 

decimal 
	UnitPrice 
{ 
get "
;" #
set$ '
;' (
}) *
public 

virtual 
Purchase 
Purchase $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

virtual 
Product 
Product "
{# $
get% (
;( )
set* -
;- .
}/ 0
} É
|C:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\Purchase.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
[ 
Table 
( 
$str 
) 
] 
public 
class 
Purchase 
: 
AuditableEntity '
{ 
[		 
Key		 
]		 	
public

 

int

 
Id

 
{

 
get

 
;

 
set

 
;

 
}

 
[ 
Column 
( 
TypeName 
= 
$str '
)' (
]( )
public 

decimal 
TotalPurchase  
{! "
get# &
;& '
set( +
;+ ,
}- .
[ 

ForeignKey 
( 
$str 
) 
] 
public 

short 

SupplierId 
{ 
get !
;! "
set# &
;& '
}( )
public 

virtual 
Supplier 
Supplier $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

virtual 
ICollection 
< 
PurchaseDetail -
>- .
PurchaseDetails/ >
{? @
getA D
;D E
setF I
;I J
}K L
} å
áC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\ProductPriceHistory.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
[ 
Table 
( 
$str 
) 
]  
public 
class 
ProductPriceHistory  
{ 
public 

short 
	ProductId 
{ 
get  
;  !
set" %
;% &
}' (
public		 

short		 

SupplierId		 
{		 
get		 !
;		! "
set		# &
;		& '
}		( )
public

 

int

 

PurchaseId

 
{

 
get

 
;

  
set

! $
;

$ %
}

& '
[ 
Column 
( 
TypeName 
= 
$str '
)' (
]( )
public 

decimal 
PurchasePrice  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

DateTime 
PurchaseDate  
{! "
get# &
;& '
set( +
;+ ,
}- .
public 

virtual 
Product 
Product "
{# $
get% (
;( )
set* -
;- .
}/ 0
public 

virtual 
Supplier 
Supplier $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

virtual 
Purchase 
Purchase $
{% &
get' *
;* +
set, /
;/ 0
}1 2
} Ô
{C:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\Product.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
[ 
Table 
( 
$str 
) 
] 
public 
class 
Product 
: 
AuditableEntity &
{ 
[		 
Key		 
]		 	
[

 
DatabaseGenerated

 
(

 #
DatabaseGeneratedOption

 .
.

. /
None

/ 3
)

3 4
]

4 5
public 

short 
Id 
{ 
get 
; 
set 
; 
}  !
[ 
Required 
] 
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
public 

string 
? 
Description 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

short 

TotalStock 
{ 
get !
;! "
set# &
;& '
}( )
[ 

ForeignKey 
( 
$str 
) 
] 
public 

byte 

CategoryId 
{ 
get  
;  !
set" %
;% &
}' (
public 

virtual 
Category 
Category $
{% &
get' *
;* +
set, /
;/ 0
}1 2
public 

virtual 
ICollection 
< 
SupplierProduct .
>. /
SupplierProducts0 @
{A B
getC F
;F G
setH K
;K L
}M N
public 

virtual 
ICollection 
< 
PurchaseDetail -
>- .
PurchaseDetails/ >
{? @
getA D
;D E
setF I
;I J
}K L
} ˆ

|C:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\Category.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
[ 
Table 
( 
$str 
) 
] 
public 
class 
Category 
: 
AuditableEntity '
{ 
[		 
Key		 
]		 	
public

 

byte

 
Id

 
{

 
get

 
;

 
set

 
;

 
}

  
[ 
Required 
] 
[ 
	MaxLength 
( 
$num 
) 
] 
public 

string 
Name 
{ 
get 
; 
set !
;! "
}# $
public 

string 
? 
Description 
{  
get! $
;$ %
set& )
;) *
}+ ,
public 

virtual 
ICollection 
< 
Product &
>& '
Products( 0
{1 2
get3 6
;6 7
set8 ;
;; <
}= >
} †
ÉC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Entities\AuditableEntity.cs
	namespace 	
InventoryManagement
 
. 
Domain $
.$ %
Entities% -
;- .
public 
abstract 
class 
AuditableEntity %
{ 
[ 
Column 
( 
$str 
) 
]  
public 

short 
? 
CreatedByUserId !
{" #
get$ '
;' (
set) ,
;, -
}. /
[		 
Column		 
(		 
$str		 
)		 
]		 
public

 

DateTime

 
CreationDate

  
{

! "
get

# &
;

& '
set

( +
;

+ ,
}

- .
[ 
Column 
( 
$str 
)  
]  !
public 

DateTime 
ModificationDate $
{% &
get' *
;* +
set, /
;/ 0
}1 2
[ 
Column 
( 
$str 
) 
] 
public 

byte 
Status 
{ 
get 
; 
set !
;! "
}# $
public 

virtual 
User 
? 
CreatedByUser &
{' (
get) ,
;, -
set. 1
;1 2
}3 4
=5 6
null7 ;
!; <
;< =
} ˛
qC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Domain\Class1.cs
	namespace 	
Domain
 
; 
public 
class 
Class1 
{ 
} ç
âC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Controllers\WeatherForecastController.cs
	namespace 	
InventoryManagement
 
. 
Controllers )
;) *
[ 
ApiController 
] 
[ 
Route 
( 
$str 
) 
] 
public 
class %
WeatherForecastController &
:' (
ControllerBase) 7
{ 
private		 
static		 
readonly		 
string		 "
[		" #
]		# $
	Summaries		% .
=		/ 0
new		1 4
[		4 5
]		5 6
{

 
$str 
, 
$str 
, 
$str '
,' (
$str) /
,/ 0
$str1 7
,7 8
$str9 ?
,? @
$strA H
,H I
$strJ O
,O P
$strQ ]
,] ^
$str_ j
} 
; 
private 
readonly 
ILogger 
< %
WeatherForecastController 6
>6 7
_logger8 ?
;? @
public 
%
WeatherForecastController $
($ %
ILogger% ,
<, -%
WeatherForecastController- F
>F G
loggerH N
)N O
{ 
_logger 
= 
logger 
; 
} 
[ 
HttpGet 
( 
Name 
= 
$str (
)( )
]) *
public 

IEnumerable 
< 
WeatherForecast &
>& '
Get( +
(+ ,
), -
{ 
return 

Enumerable 
. 
Range 
(  
$num  !
,! "
$num# $
)$ %
.% &
Select& ,
(, -
index- 2
=>3 5
new6 9
WeatherForecast: I
{ 	
Date 
= 
DateOnly 
. 
FromDateTime (
(( )
DateTime) 1
.1 2
Now2 5
.5 6
AddDays6 =
(= >
index> C
)C D
)D E
,E F
TemperatureC 
= 
Random !
.! "
Shared" (
.( )
Next) -
(- .
-. /
$num/ 1
,1 2
$num3 5
)5 6
,6 7
Summary 
= 
	Summaries 
[  
Random  &
.& '
Shared' -
.- .
Next. 2
(2 3
	Summaries3 <
.< =
Length= C
)C D
]D E
} 	
)	 

. 	
ToArray	 
( 
) 
; 
} 
}   õ
C:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Controllers\UsersController.cs
[ 
ApiController 
] 
[ 
Route 
( 
$str 
) 
] 
public 
class 
UsersController 
: 
ControllerBase -
{		 
private

 
readonly

 
InventoryDbContext

 '
_context

( 0
;

0 1
public 

UsersController 
( 
InventoryDbContext -
context. 5
)5 6
{ 
_context 
= 
context 
; 
} 
[ 
HttpPost 
] 
public 

async 
Task 
< 
IActionResult #
># $

CreateUser% /
(/ 0
[0 1
FromBody1 9
]9 :
CreateUserDto; H
userDtoI P
)P Q
{ 
if 

( 
userDto 
== 
null 
) 
{ 	
return 

BadRequest 
( 
) 
;  
} 	
var 
newUser 
= 
new 
User 
{ 	
Username 
= 
userDto 
. 
Username '
,' (
PasswordHash 
= 
userDto "
." #
Password# +
,+ ,
	FirstName 
= 
userDto 
.  
	FirstName  )
,) *
LastName   
=   
userDto   
.   
LastName   '
,  ' (
SecondLastName!! 
=!! 
userDto!! $
.!!$ %
SecondLastName!!% 3
,!!3 4
Role"" 
="" 
userDto"" 
."" 
Role"" 
,""  
Status## 
=## 
$num## 
,## 
CreationDate$$ 
=$$ 
DateTime$$ #
.$$# $
UtcNow$$$ *
,$$* +
ModificationDate%% 
=%% 
DateTime%% '
.%%' (
UtcNow%%( .
,%%. /
}&& 	
;&&	 

await(( 
_context(( 
.(( 
Users(( 
.(( 
AddAsync(( %
(((% &
newUser((& -
)((- .
;((. /
await)) 
_context)) 
.)) 
SaveChangesAsync)) '
())' (
)))( )
;))) *
return++ 
Ok++ 
(++ 
newUser++ 
)++ 
;++ 
},, 
}-- í
ÇC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Controllers\ProductsController.cs
	namespace 	
InventoryManagement
 
. 
Controllers )
;) *
[ 
ApiController 
] 
[ 
Route 
( 
$str 
) 
] 
public		 
class		 
ProductsController		 
:		  !
ControllerBase		" 0
{

 
} á
çC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Application\Interfaces\IProductRepository.cs
	namespace 	
InventoryManagement
 
. 
Application )
.) *

Interfaces* 4
;4 5
public 
	interface 
IProductRepository #
{ 
} Ê
C:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Application\DTOs\ProductDto.cs
	namespace 	
InventoryManagement
 
. 
Application )
.) *
DTOs* .
;. /
public 
class 

ProductDto 
{ 
} ß

ÇC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Application\DTOs\CreateUserDto.cs
	namespace 	
InventoryManagement
 
. 
Application )
.) *
DTOs* .
;. /
public 
class 
CreateUserDto 
{ 
public 

string 
Username 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
Password 
{ 
get  
;  !
set" %
;% &
}' (
public 

string 
	FirstName 
{ 
get !
;! "
set# &
;& '
}( )
public 

string 
LastName 
{ 
get  
;  !
set" %
;% &
}' (
public		 

string		 
?		 
SecondLastName		 !
{		" #
get		$ '
;		' (
set		) ,
;		, -
}		. /
public

 

string

 
Role

 
{

 
get

 
;

 
set

 !
;

! "
}

# $
} à
vC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Application\Class1.cs
	namespace 	
Application
 
; 
public 
class 
Class1 
{ 
} 