create table Articles
    ( Id INT PRIMARY KEY autoincrement,
      Title string,
      Source string,
      Parsed string,
      CreatedOn datetime
    );

create table Tags
    ( Id INT PRIMARY KEY autoincrement,
      Name string
    );


create table ArticleTags
    ( ArticleId INT REFERENCES Articles(Id),
      TagId INT REFERENCES Tags(Id)
    );
