CREATE TABLE articles
(
    id          integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    title       text        NOT NULL,
    "source"    text        NOT NULL,
    parsed      text        NOT NULL,
    createdon   timestamptz NOT NULL,
    description text        NOT NULL
);

CREATE TABLE tags
(
    id     integer PRIMARY KEY GENERATED ALWAYS AS IDENTITY,
    "name" text NOT NULL,
    CONSTRAINT tags_name_unique UNIQUE (name)
);

CREATE TABLE article_tags
(
    articleid integer NOT NULL REFERENCES articles (id),
    tagid     integer NOT NULL REFERENCES tags (id)
);
