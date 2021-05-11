# Kálmán Richárd (JSRFDB)

## Fejlesztői környezetek

- Microsoft Visual Studio 2019
- Microsoft Visual Studio Code

## Előfeltételek

- .NET 5.0 verzio
- Node.js, NPM csomagkezelés miatt.
- Microsoft Visual Studio 2019

## Telepítés

Töltsük le vagy klónozzuk le a repositoryt
=======
Töltsük vagy klónozzuk a repositoryt

### Backend elindítása

./Server mappában található indítsuk el .sln fájlt és futtasuk a programot. 

### Frontend elindítása

./Client mappában nyissuk egy terminált/cmd és futtasuk a következő parancsokat

1. **npm install**  ez a package-ek telepítésért felelős, leszed az internetről minden függőséget
2. ### **npm start** frontend indítása. 

#### Alapértelmezett elérési utak, portok és változtatása


2. **npm start** frontend indítása. 

#### Alapértelmezett elérési utak, portok és változtatása

Abban az esetben, ha nem külön domainen fut a két szerver, akkor Cors error-ok elkerülése érdekében figyelemben kell venni a port számokat.

**Frontendnek** a localhost:3000  linken kell elérhető, ha port különbözik, akkor a szerver gyökérkönyvtárában található startup.cs fájlban átkell írni a portot.

  ![](https://github.com/RichardKalman/React-Asp.net-Todo-List/blob/main/Image/serverport.PNG)

**Backendnek** a localhost:5000 linken kell elérhető lennie, ha esetleg más porton érhető vagy másik URL-en, akkor megkell változtatni a frontenden.

Ezt a ./client/.env fájlban tudod megtenni a REACT_APP_API_URL átírásával

Mind a kettő esetben a újra kell indítani a módosított programot. 

Ha mindent jól csináltuk az applikáció használatra készen áll. 

## Frontend felépítése

A project ./client mappába került. A komponensek a ./src/Components mappában találhatóak. Az oldal dizájnja a boostrap könyvtár használatával készült. 

### Az alkalmazás 3 részre lehet bontani, ami kisebb komponensekre van bontva. 

1.  Navigációs sáv:  Components mappán belül Navbar/NavbarComponent.js fájl tartalmazza ezt a rész
   - Ez a navigációs sáv kirajzolásáért felelős. Minden oldalon megjelenik.
2. Teendők: Components mappán belül a Todo mappában található a komponensei
   - Ez a rész a táblák kirajzolásáért és DragAndDrop, Új teendő felvétel, Teendő törlése
   - Kompenensek és felelőségei:
     1. DragDropComponent:  A teendők és táblák kirajzolásért és funkcionalitásáért ez a komponenst felel, használja fel az alábbi komponenseket. Példányosodásakor betölti a szerverről a fájlokat, ha elengedünk egy mozgatható elemet kezeli a eseményeket. Módosításokat küld a szerver felé. 
        1.   A CardComponent: a táblák kirajzolásáért felel és Droppable helyek definiálása. DragDropComponent hívja meg, annyiszor amennyi táblánk van
        2.  ItemComponent: a Draggable elemek kirajzolásáért felelős.
     2.  Az új teendő felvétel és teendő törlés azok Modal ablakokkal lehetségesek. Ezek kirajzolásáért és funkcionalitásáért a  DeleteTodoModalComponent és a NewTodoModalComponent felelős
3. Teendők típus: Ez a rész a Teendők táblájáért felelős. Komponensei megtalálhatóak a TodoType mappába. 
   1. TodoTypeComponent: A táblák kilistázásáért , adat struktúra összeállításáért, Módosítások küldéséért felelős. 
   2. NewTodoTypeModalCompenent és DeleteTodoTypeComponent: a Tábla létrehozásáért és törléséért felelős komponensek. Mind a kettő komponens egy modal ablakkal tér vissza.

### Használt adatstruktúrák

1. DragDropComponent:

   ![](https://github.com/RichardKalman/React-Asp.net-Todo-List/blob/main/Image/dragdropstruct.PNG) 

2. TodoTypeComponent:

   ![](https://github.com/RichardKalman/React-Asp.net-Todo-List/blob/main/Image/todotypestruck.PNG)



## Backend felépítése

Asp.net 5.0 Api projectben van létrehozva. Az adatkezelés SQLite adatbázissal és Entityframework segítségével valósul meg.

3 különböző namespaceből áll az alkalmazás

- TodosApplication.Model
- TodosApplication.DAL
- TodosApplication.Controllers

### TodosApplication.Model

Ebben a namespaceben található a 2 darab entitást, amilyen tábla van az adatbázisban is.

**Todo entitás tulajdonságai:** 

- Id:  Integer típusú automatikusan generált.
- TypeId: Integer típusú hozzátartozó Type (tábla) id-ja minden Todo-nak kötelező lennie. 
- Name: String típusú. A Todo nevét tárolja
-  Details: String típusú. A Todo leírását tárolja
-  Deadline: Értelemszerűen azt tárolja ami a neve. 
- Type: Todotype típusú a hozzátartozó Type összes adata.
- Order: Integeren típusú, sorrend meghatározására használt tulajdonság. Mindig frissül ha az egyik táblából a másikban helyezzünk egy Todo-t és/vagy a sorrendet változtatjuk

**TodoType entitás tulajdonságai:**

- Id: Integer típusú, automatikusan generált
- Name: String típusú. A tábla nevét tárolja
- Order: Integer típusú, sorrend meghatározására használt tulajdonság

### TodosApplication.DAL

Ebben a namespaceben 2 db fájl található. Egy Interface ami a tesztelhetőség miatt kell és egy class ami az adatelérést felel.

**TodoContext**

Az adatbázis eléréséért felel. Kettő DbSet tulajdonsággal, az egyik Todo típusú a másik TodoType. Erre a két változónak hivatkozásával érjük el az adatbázisban tárolt elemek

OnModelCreating függvény a migráció létrehozásáért felelős függvény. 

### TodosApplication.Controllers

Két darab controllert tartalmaz, Egy a Todo-ért felelős TodoController, a másik TodoType-ért (Táblák) TodoTypeController

**TodoController.cs**

Végpontjai:

| Kérés típusa |           URL           |                           Leírás                            |           JSON Adat            |     Visszatérési érték     |
| :----------: | :---------------------: | :---------------------------------------------------------: | :----------------------------: | :------------------------: |
|     GET      |        /api/Todo        | Lekéri az összes teendőt, order szerint növekvő rendezéssel |               -                |     IEnumerable<Todo>      |
|     POST     |        /api/Todo        |                   Hozzáad egy új teendőt                    |   mezo,deadline,name,details   | Az új teendővel tér vissza |
|    DELETE    |        /api/Todo        |                      Töröl id szerint                       |               id               |         HTTP code          |
|     PUT      | /api/Todo/toothercolumn |        Áthelyezz egy másik táblába egy adott teendőt        |   Item, Destinationid, index   |         HTTP code          |
|     PUT      |     /api/Todo/sort      |              Egy teendő rendezése táblán belül              | item,srcindex destinationindex |         HTTP code          |

Ezeken kívül kettő darab függvény van benne: 

|         Név         |                 Bementei paraméterek                 |                         Leírás                          |
| :-----------------: | :--------------------------------------------------: | :-----------------------------------------------------: |
| ReOrderTodosRowSort |  List<Todo> todos,int srcindex,int destinationindex  | átrakja a Todot a másik helyre és újra rendezi a táblát |
|    ReOrderTodos     | Queryable<Todo> todos, int order= 0, Todo todo = nul |                  Újra rendezi a táblát                  |

**TodoTypeController**



| Kérés típusa |          URL          |                            Leírás                            |         JSON Adat          |     Visszatérési érték     |
| :----------: | :-------------------: | :----------------------------------------------------------: | :------------------------: | :------------------------: |
|     GET      |     /api/TodoType     | Lekéri az összes teendőtáblát, order szerint növekvő rendezéssel |             -              |   IEnumerable<TodoType>    |
|     POST     |     /api/TodoType     |                 Hozzáad egy új teendőtáblát                  |            name            | Az új teendővel tér vissza |
|    DELETE    |     /api/TodoType     | Töröl id szerint, és kitörli az összes Todo elemet ami hozzátartozik |             id             |         HTTP code          |
|     PUT      | /api/TodoType/rowSort |        Áthelyezz egy másik táblába egy adott teendőt         | Item, Destinationid, index |         HTTP code          |



Test projektben pedig az egy szerény teszt-et tartalmazza.
=======





