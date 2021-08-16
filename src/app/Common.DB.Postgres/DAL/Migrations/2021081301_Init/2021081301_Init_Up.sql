CREATE TABLE public."Users"
(
    "Id" uuid NOT NULL,
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
    "Id" uuid NOT NULL,
    "Type" int NOT NULL,
    "Value" text,
    "ExpireAt" timestamp without time zone NOT NULL,
    "UserId" uuid,
    PRIMARY KEY ("Id")
);

ALTER TABLE public."Tokens"
    OWNER to postgres;