# הודעות לתושב

מערכת להצגת הודעות ועדכונים לתושבים. המאגר כולל ממשק Web בעברית ובכיווניות RTL, לצד API שמחזיר הודעות מסוננות ומנורמלות לפי קוד רשות מקומית.

## רכיבי המערכת

- **Client** — אפליקציית Angular המציגה הודעות, מסמנת הודעות דחופות ומטפלת במצב טעינה ריק או בכשל טעינה.
- **API** — שירות ASP.NET Core המספק נקודת קצה לקריאת הודעות לפי רשות.
- **לוגיקת שירות ותשתית** — סינון ונרמול נתוני ההודעות, ממשקי גישה לנתונים ומימושי Mock לצורכי פיתוח והדגמה.

## טכנולוגיות

- **Client:** Angular 18, TypeScript, SCSS, Angular HttpClient ו־RxJS.
- **Server:** .NET 8, ASP.NET Core Web API, C# ו־Swagger/OpenAPI בסביבת Development.
- **בדיקות שרת:** xUnit.

## דרישות מקדימות

- Node.js ו־npm, בגרסאות התואמות ל־Angular CLI 18.
- .NET SDK 8.
- דפדפן מודרני.

## הרצה מקומית

### 1. הפעלת השרת

מתיקיית המאגר הראשית:

```bash
dotnet run --project Ra_MuniAnnouncment/RA_MuniAnnouncements.csproj --launch-profile https
```

פרופיל `https` מפעיל את השרת בכתובות `https://localhost:7209` , ופותח את Swagger בסביבת Development. אם אישור הפיתוח של HTTPS אינו מותקן או מהימן, יש להגדיר אותו באמצעות כלי .NET (`dotnet dev-certs https`).

### 2. הפעלת הממשק

בחלון טרמינל נוסף, מתוך תיקיית `client`:

```bash
npm install
npm start
```

הממשק יהיה זמין ב־[http://localhost:4200](http://localhost:4200). כתובת ה־API מוגדרת ב־`client/src/app/announcements/announcement.service.ts`, ומדיניות CORS בשרת מאפשרת כרגע את מקור הפיתוח `http://localhost:4200`.

## API

### קבלת הודעות לרשות

```http
GET /api/announcements/{authorityCode}
```

לדוגמה:

```text
https://localhost:7209/api/announcements/6500
```

במקרה הצלחה מוחזרת תשובת JSON עם `authorityCode`, `authorityName` ומערך `announcements`. כל הודעה כוללת מזהה, כותרת, תקציר, שם קטגוריה, סימון דחיפות ותאריך פרסום. השרת מסנן הודעות שאינן מפורסמות, הודעות מחוץ לטווח התאריכים וקטגוריות שאינן מופעלות; הוא מסיר HTML מהתקציר, מגביל אותו ל־150 תווים ומחזיר הודעות דחופות תחילה.

קוד הרשות שהממשק משתמש בו מוגדר כרגע ל־`6500` ב־`client/src/app/app.component.ts`. השרת מחזיר `404` כאשר לא קיימות הגדרות לרשות המבוקשת, ו־`400` כאשר קוד הרשות ריק.

## נתוני פיתוח

השרת רשום כרגע להשתמש ב־`CmsClientMock` וב־`SettingsRepositoryMock`, עם נתונים והגדרות בזיכרון עבור רשות 6500. אלו מימושי הדגמה; חיבור למערכת CMS או למאגר הגדרות אמיתי דורש מימושים מתאימים ל־`ICmsClient` ול־`ISettingsRepository` ועדכון רישום התלויות ב־`Ra_MuniAnnouncment/Program.cs`.

## בדיקות ובנייה

בדיקות השרת מתוך תיקיית המאגר הראשית:

```bash
dotnet test ResidentAnnouncements.sln
```

פקודות ה־Client מתוך תיקיית `client`:

```bash
npm run build   # בניית האפליקציה
npm run watch   # בנייה חוזרת בכל שינוי
npm test        # בדיקות Angular
```

תוצרי הבנייה של Angular נשמרים ב־`client/dist/`.

## מבנה המאגר

```text
ResidentAnnouncements/
├── Ra_MuniAnnouncment/                 # ASP.NET Core API, Controllers, Middleware ו־Program.cs
├── Ra_MuniAnnouncment.Core/            # מודלים וישויות
├── Ra_MuniAnnouncment.Infrastructure/  # ממשקי ומימושי גישה לנתונים (כולל Mock)
├── Ra_MuniAnnouncment.Service/        # לוגיקה עסקית וממשק השירות
├── Ra_MuniAnnouncment.test/           # בדיקות xUnit
├── ResidentAnnouncements.sln          # פתרון .NET
└── client/                             # אפליקציית Angular
    └── src/app/announcements/          # שירות API ורכיב תצוגת ההודעות
```

## פיתוח והגדרות

- כתובת ה־API וקוד הרשות בצד הלקוח מוגדרים בקובצי המקור המוזכרים לעיל.
- כתובות ההאזנה של שרת הפיתוח נמצאות ב־`Ra_MuniAnnouncment/Properties/launchSettings.json`.
- Swagger מופעל רק בסביבת Development.
- מדיניות CORS הנוכחית מותאמת להרצה מקומית של Angular ב־`localhost:4200`; יש להתאים אותה למקור הממשק בפריסה.
