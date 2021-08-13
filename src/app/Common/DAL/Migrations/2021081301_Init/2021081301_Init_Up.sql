CREATE TABLE public."Users"
(
    "Id" bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    "FirstName" text,
    "LastName" text,
    "PasswordHash" text,
    "Email" text,
    "IsEmailVerified" boolean NOT NULL,
    "SignupToken" text,
    "LastRequest" timestamp without time zone NOT NULL,
    "OAuthGoogle" boolean NOT NULL,
    "ResetPasswordToken" text,
    PRIMARY KEY ("Id")
);

ALTER TABLE public."Users"
    OWNER to postgres;

CREATE TABLE public."Tokens"
(
    "Id" bigint NOT NULL GENERATED ALWAYS AS IDENTITY,
    "Type" int NOT NULL,
    "Value" text,
    "ExpireAt" timestamp without time zone NOT NULL,
    "UserId" bigint,
    PRIMARY KEY ("Id")
);

ALTER TABLE public."Tokens"
    OWNER to postgres;