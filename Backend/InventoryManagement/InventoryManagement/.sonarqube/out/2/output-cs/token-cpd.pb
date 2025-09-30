º
‘C:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Infrastructure\Repositories\ProductRepository.cs
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
} ¶
‘C:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Infrastructure\Persistence\InventoryDbContext.cs
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
}   Ž
yC:\Users\Adrian\source\repos\InventoryManagement\Backend\InventoryManagement\InventoryManagement\Infrastructure\Class1.cs
	namespace 	
Infrastructure
 
; 
public 
class 
Class1 
{ 
} 