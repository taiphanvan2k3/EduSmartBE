--
-- PostgreSQL database cluster dump
--

\restrict RcTWqvmf4JBDEDX0iRvueRzFvPOksP9gb2vEm7Av4IaEx8BLE8KyFqnjtmwgs7q

SET default_transaction_read_only = off;

SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;

--
-- Roles
--

CREATE ROLE pbl6duter;
ALTER ROLE pbl6duter WITH SUPERUSER INHERIT CREATEROLE CREATEDB LOGIN REPLICATION BYPASSRLS PASSWORD 'SCRAM-SHA-256$4096:AwRqOAp+6llMveBKa0fttg==$AIZY1INFkFwN5caS3U0hBO5HAkO9U1WCrkf8JNAoj5c=:lqymg3YnA9muYQSdriH9vstEab0NqHmpY2jisSTcDUI=';

--
-- User Configurations
--








\unrestrict RcTWqvmf4JBDEDX0iRvueRzFvPOksP9gb2vEm7Av4IaEx8BLE8KyFqnjtmwgs7q

--
-- Databases
--

--
-- Database "template1" dump
--

\connect template1

--
-- PostgreSQL database dump
--

\restrict fHzdJx0fS8Aww67r88wmmT2W77gpw5Iz5SItlGkPiet3LWGYZUjYS8ulHRe6roA

-- Dumped from database version 17.10 (Debian 17.10-1.pgdg13+1)
-- Dumped by pg_dump version 17.10 (Debian 17.10-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- PostgreSQL database dump complete
--

\unrestrict fHzdJx0fS8Aww67r88wmmT2W77gpw5Iz5SItlGkPiet3LWGYZUjYS8ulHRe6roA

--
-- Database "EduSmart.AuthService" dump
--

--
-- PostgreSQL database dump
--

\restrict RUR7F6Ofwap8COTfbLu7uVGQMiiyDl5PxcTNX5OhnY8c64ifAfWPu0QkisYaxuI

-- Dumped from database version 17.10 (Debian 17.10-1.pgdg13+1)
-- Dumped by pg_dump version 17.10 (Debian 17.10-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: EduSmart.AuthService; Type: DATABASE; Schema: -; Owner: pbl6duter
--

CREATE DATABASE "EduSmart.AuthService" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE "EduSmart.AuthService" OWNER TO pbl6duter;

\unrestrict RUR7F6Ofwap8COTfbLu7uVGQMiiyDl5PxcTNX5OhnY8c64ifAfWPu0QkisYaxuI
\connect "EduSmart.AuthService"
\restrict RUR7F6Ofwap8COTfbLu7uVGQMiiyDl5PxcTNX5OhnY8c64ifAfWPu0QkisYaxuI

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: CoursePermissions; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."CoursePermissions" (
    "CourseId" integer NOT NULL,
    "AssistantId" integer NOT NULL,
    "FunctionId" character varying(20) NOT NULL,
    "IsEnable" boolean NOT NULL
);


ALTER TABLE public."CoursePermissions" OWNER TO pbl6duter;

--
-- Name: Functions; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Functions" (
    "Id" character varying(20) NOT NULL,
    "Code" character varying(100),
    "Name" character varying(150),
    "Order" integer NOT NULL,
    "ScreenId" character varying(20),
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."Functions" OWNER TO pbl6duter;

--
-- Name: Permissions; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Permissions" (
    "RoleId" integer NOT NULL,
    "FunctionId" character varying(20) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."Permissions" OWNER TO pbl6duter;

--
-- Name: RefreshTokens; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."RefreshTokens" (
    "Id" uuid NOT NULL,
    "Token" character varying(100),
    "IPAddress" character varying(40),
    "UserId" integer NOT NULL,
    "Expires" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "RevokedAt" timestamp with time zone,
    "IsRevoked" boolean DEFAULT false NOT NULL
);


ALTER TABLE public."RefreshTokens" OWNER TO pbl6duter;

--
-- Name: RoleClaims; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."RoleClaims" (
    "Id" integer NOT NULL,
    "RoleId" integer NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."RoleClaims" OWNER TO pbl6duter;

--
-- Name: RoleClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."RoleClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."RoleClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Roles; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Roles" (
    "Id" integer NOT NULL,
    "Name" character varying(256),
    "NormalizedName" character varying(256),
    "ConcurrencyStamp" text
);


ALTER TABLE public."Roles" OWNER TO pbl6duter;

--
-- Name: Roles_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."Roles" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Roles_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Screens; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Screens" (
    "Id" character varying(20) NOT NULL,
    "Code" character varying(100),
    "Name" character varying(150),
    "Order" integer NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."Screens" OWNER TO pbl6duter;

--
-- Name: UserClaims; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."UserClaims" (
    "Id" integer NOT NULL,
    "UserId" integer NOT NULL,
    "ClaimType" text,
    "ClaimValue" text
);


ALTER TABLE public."UserClaims" OWNER TO pbl6duter;

--
-- Name: UserClaims_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."UserClaims" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."UserClaims_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: UserLogins; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."UserLogins" (
    "LoginProvider" text NOT NULL,
    "ProviderKey" text NOT NULL,
    "ProviderDisplayName" text,
    "UserId" integer NOT NULL
);


ALTER TABLE public."UserLogins" OWNER TO pbl6duter;

--
-- Name: UserRoles; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."UserRoles" (
    "UserId" integer NOT NULL,
    "RoleId" integer NOT NULL
);


ALTER TABLE public."UserRoles" OWNER TO pbl6duter;

--
-- Name: UserTokens; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."UserTokens" (
    "UserId" integer NOT NULL,
    "LoginProvider" text NOT NULL,
    "Name" text NOT NULL,
    "Value" text
);


ALTER TABLE public."UserTokens" OWNER TO pbl6duter;

--
-- Name: Users; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Users" (
    "Id" integer NOT NULL,
    "FirstName" text,
    "LastName" text,
    "AvatarURL" text,
    "Phone" text,
    "Gender" integer NOT NULL,
    "IsOnline" boolean NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UserName" character varying(256),
    "NormalizedUserName" character varying(256),
    "Email" character varying(256),
    "NormalizedEmail" character varying(256),
    "EmailConfirmed" boolean NOT NULL,
    "PasswordHash" text,
    "SecurityStamp" text,
    "ConcurrencyStamp" text,
    "PhoneNumber" text,
    "PhoneNumberConfirmed" boolean NOT NULL,
    "TwoFactorEnabled" boolean NOT NULL,
    "LockoutEnd" timestamp with time zone,
    "LockoutEnabled" boolean NOT NULL,
    "AccessFailedCount" integer NOT NULL
);


ALTER TABLE public."Users" OWNER TO pbl6duter;

--
-- Name: Users_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."Users" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Users_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Data for Name: CoursePermissions; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."CoursePermissions" ("CourseId", "AssistantId", "FunctionId", "IsEnable") FROM stdin;
\.


--
-- Data for Name: Functions; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Functions" ("Id", "Code", "Name", "Order", "ScreenId", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: Permissions; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Permissions" ("RoleId", "FunctionId", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: RefreshTokens; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."RefreshTokens" ("Id", "Token", "IPAddress", "UserId", "Expires", "CreatedAt", "RevokedAt", "IsRevoked") FROM stdin;
b39a79a9-80b6-46c2-80f4-e7d15f6f046e	//YupuRPe7bWAqcQvb+PJbciGTJkO/Vas3GabC+N6FmdWvQqSvXHMqS4R5qHBFBnUROoynjp49GY7+gTHE+xXg==	192.168.32.9	2	2026-07-14 08:46:15.082939+00	2026-07-07 08:46:15.082955+00	\N	f
ffd33d0c-d163-4425-b14a-49000cc6a8f5	1mXX0DunBpOvfKfaZgo0knUcJKIs4m6g5GDgIQsJOqyQ45ezkwtHsmh1/0JqLGLbGgjfRngF1GJPwNia3MXUCA==	192.168.32.9	3	2026-07-14 08:48:04.648695+00	2026-07-07 08:48:04.648699+00	\N	f
b2a6ab1a-786d-457e-80f4-581fa80c5101	V1muXyIAVrBT+3uSZ/55/XLYJnvCMzjxoPNaxSMhmx08ScMq4sYqHLMkwOTWHX7Vte14c5FTSlLLbTmVWRO3QQ==	192.168.32.9	2	2026-07-14 09:07:43.796777+00	2026-07-07 09:07:43.796777+00	\N	f
11da16d2-dee9-42af-9524-6cc33edc4d54	16UNJ8LE7lAaLeRYQVWoNC8c+rNNlRwSo4JWl80r7U/lVDutp1s2czaoqtIes9hMGNVHO1XqOLQxpjMs5m0xew==	192.168.32.9	2	2026-07-14 09:47:47.720561+00	2026-07-07 09:47:47.720568+00	\N	f
99c38b13-f5c0-4e44-8042-bb705e2fe166	LgE6GPDQnAeCnVhF5UOSbAJ8dfA4BKpN6I/lz+nU7EdyUytC1MNCV0RI3Wv+MyIRL1hNMLZuDcxjQvynNOFMzQ==	192.168.32.9	4	2026-07-14 09:52:10.630985+00	2026-07-07 09:52:10.630988+00	\N	f
0c288a56-442b-4611-9aa0-ef214d21610b	qYT4VvHURCHmEE2PsBzQPQeJr5YLs96ovfvqFH4GhuT21iyeAKwtVFAL2WCZxxXnBoo54+lBGVg0aXYMiKpHYg==	192.168.32.9	3	2026-07-14 09:52:39.277229+00	2026-07-07 09:52:39.27723+00	\N	f
518ea9ab-94ab-4623-94da-68672a8bd924	kFEcSAdBYE2B2eQ/DNw1gsNg0WSASvvUTgnhHaOHN8RRMiWIyailkX32unXrdd4wxVudTorVAtMsrQYC+m4QtA==	192.168.32.9	3	2026-07-14 09:59:30.81493+00	2026-07-07 09:59:30.814934+00	\N	f
8315374a-5416-486c-98a5-303d46b31c68	7D1vqxyUvSsyrMvU4UPfn8xXsPxgkWDwdEWkj9+VeRlXgGvMiORAFoQh0CNOQfysUV0IjXC1Mid4hXvJvlONtw==	192.168.32.9	5	2026-07-14 10:01:02.016391+00	2026-07-07 10:01:02.016392+00	\N	f
f7295cec-fd6c-40a8-ad71-0f4bb94c903d	gA2yznI+IuBV5hTegeJs7NHHo/z7PJRd7aGpzjbw/KIbN4MB0ylZg+hnZkLO953/sCcZ+bW+hM8vYMKXyCuACw==	192.168.32.9	6	2026-07-14 10:11:58.742123+00	2026-07-07 10:11:58.742124+00	2026-07-07 15:39:15.841055+00	t
5aeb374d-89a5-4e9d-8cb2-e370cf1a4942	p/HWl5sr42TvTu+kbmmCS/RD1VmyL/FlDRxkyZzFTswq4cmtTULxljrRRBqaVDMChp38MZuPCuG8irNORtO3OQ==	192.168.32.9	6	2026-07-14 15:39:15.841097+00	2026-07-07 15:39:15.841098+00	\N	f
cadc9ae0-5d42-4d56-8f0c-e9375365fc5f	uHlrEOTgrIQC9mC/fZUHh1MpwgX5Xq8G+FIx8ijAfge+UFcdWzbqk3alWtmGaXoUDxw1rEqe3PAsrAAbcUacMw==	192.168.32.9	3	2026-07-14 15:39:39.143234+00	2026-07-07 15:39:39.143237+00	\N	f
31e1293e-18f8-4f74-9910-8551c6734c1b	oJIZRwBB1d7EXNwVx5af4oBXhTg60y99GyUGPJLmwsFD0A8CKaLevUbGEIPpOA20soYcWUEzeXE8IadejhH5+w==	192.168.32.9	3	2026-07-14 09:11:15.427412+00	2026-07-07 09:11:15.427415+00	2026-07-07 16:25:34.002391+00	t
6012fb55-6a9a-4012-960d-c29be149ed17	7j/+vzJ4L+iSX1YX9jmtqIh5kHJtpsWh1anPghssDPTfv9LZOo6Npf6WlK8jpl6MpWF0oi4FqYaTuK0lZy/RaA==	192.168.32.9	3	2026-07-14 16:25:34.002403+00	2026-07-07 16:25:34.002404+00	\N	f
45d88806-f6db-46f9-b38f-e6119deca20d	w3/C45ciCdxT14sMwrZPeC72ABsML6Lc0Gz68R6HNZMGgYZV5W3iI79bDmc1ptsxJ/XCaaVm9LQ/ser72Cf6PA==	192.168.32.9	6	2026-07-14 16:28:32.42634+00	2026-07-07 16:28:32.426341+00	\N	f
facf7073-3738-40b4-a5e1-82f52a78d88b	vnrUhztr6AtUyNtyk+AIgTH3dFFg4CTomaJWCb5AL2qk7RlOtf9aW8WKQKziYK9bqkagz7aoB8gLTU0flyOCug==	192.168.32.9	6	2026-07-14 15:39:51.085847+00	2026-07-07 15:39:51.085848+00	2026-07-08 02:07:07.916866+00	t
7f3cddde-81b6-4100-a3e0-a3fcf30ab7b5	+hW4cn98N/IzT4WnRpkTcELHpHRB/7lNTMNNDenB4/ENaRmruX9F0x6lMELNwRV+dQ0qtxN+i/9eXBtVhtmKWA==	192.168.32.9	6	2026-07-15 02:07:07.916884+00	2026-07-08 02:07:07.916885+00	\N	f
18f1687c-cb13-47f3-af7e-8ba0e4708c93	BauYrukjJRNrKBhDbXOLP31rTFvUdIxcItDt8bqPAJM3yr0oTuVcpIKkAE0XkVNCoM+yZGp/MT4jBUUE8mFOZw==	192.168.32.9	6	2026-07-15 02:07:33.13398+00	2026-07-08 02:07:33.133981+00	2026-07-08 06:55:44.931879+00	t
613cec39-e7e6-492a-9b68-6385ec206035	CEoS6uY34Pqi8Sj+xcSqR5sIZaru2JK9qHrJTGKwkOLpjUXciivjkrsFgsX09gLN6YVW3VWbPRxcANFlKNw4kA==	192.168.32.9	6	2026-07-15 06:55:44.931896+00	2026-07-08 06:55:44.931898+00	\N	f
12d2bf46-8cd7-40d9-810b-70d0d56cb1fe	y0rpC/KDC1j3Ta/qTyea/3Fk2ZpIKGK55sezBYQVngp79cz+AC/DXPjqnwuZAjv7NKuCsWXOMQebZdB8OlqSXA==	192.168.32.9	2	2026-07-17 04:11:01.099821+00	2026-07-10 04:11:01.099823+00	2026-07-11 11:17:04.899598+00	t
67b97570-70d9-44d7-90fc-2ccc5f2cfe6f	DhwWrqpzJg3yX5fQRWpbG7Rjz/wXEInzIf718iQWRYJXZDu/DOOIuuAWC8dPMomDhlYPJ9hrTmBUKya3OSqF5Q==	192.168.32.9	2	2026-07-18 11:17:04.899612+00	2026-07-11 11:17:04.899613+00	\N	f
22143b88-ce4e-4ce1-8010-4acf10e81784	z/wxeyCMMGUGlF4oHpwUH3TCQGGvM+Ivfb1wpYIAzFi8lIqY+On8frzlL7neyd1MuAvhdx29h79+k/t6JOECkQ==	192.168.32.9	6	2026-07-19 04:37:23.720633+00	2026-07-12 04:37:23.720633+00	\N	f
d5649ec0-c305-4081-9f63-89509497979b	/1OP8EZTPjAS+WvC1VQ+VAjwdOfNBwRbgk0qzdfPO//Nj5E/+f162v6hLaM9cbS0udRVMRsfywugTRiiDSAX0g==	192.168.32.9	4	2026-07-19 06:05:54.331233+00	2026-07-12 06:05:54.331233+00	\N	f
f4b8888f-4bcf-4ec7-b31e-5515ad83d733	hBnpXa1Qr927FgKwNfgGxJwyuEGPjeMtWKn8uKgdiRSOsSPORCEF74JgYxrF+EHg/n+mg1o4lUEzFA4eqOMFTw==	192.168.32.9	6	2026-07-18 11:17:16.027772+00	2026-07-11 11:17:16.027773+00	2026-07-12 06:24:28.851652+00	t
6e6093ac-b5b9-4689-804a-bd3998502069	KFXoHpzxocMFRLnvLSwWJixWi3QsrxUpE+IZMQpuEJuElbdmWZzqedzWuaR/L1vdGdrdblggCyJD8H+uLCiesA==	192.168.32.9	6	2026-07-19 06:24:28.851676+00	2026-07-12 06:24:28.851681+00	\N	f
545ed404-e010-42ef-abae-b44dd0100c27	7y4RpTdnhXjdCAhkHGhB3p2IUq4/knbVC+chMoPoBnTzcLSzulVXujb9JTJYmRkOwgmBdTsBgbI7/UZ2H5BS5Q==	192.168.32.9	6	2026-07-19 06:24:44.766972+00	2026-07-12 06:24:44.766972+00	\N	f
bf399cf2-dca1-45fd-818d-dc84efdb654d	Cc+Ri4bdaIAp2THFgaFVbpEv5Pzbrq0k6DAWLUAz+dl8B0PtISlnvaRz7SoG15Yv195T3yJEVYpo4cMMowzvRQ==	192.168.32.9	6	2026-07-19 06:26:12.979828+00	2026-07-12 06:26:12.979828+00	\N	f
6db5b4cb-ad89-421d-a43b-a39770fe2a45	NME8gxxTDWVpu+H+mcEhrytaiTcxkJisbiU71aDheQKiacu5MJHlPGyGDGLUCLDR/tBpgp9PfLE3rT2Z13p7vg==	192.168.32.9	2	2026-07-19 06:36:45.7234+00	2026-07-12 06:36:45.723402+00	\N	f
6f7020f3-78cd-4e08-8d67-c5dcd56134aa	11HCL/7qBVTUc5FN+W/oUYiNZkZMJPiuz6xl/vecnbz1sxu5HpZcbcYybGyV5iV6DRNHlBrvVijHscACjI2r2Q==	192.168.32.9	6	2026-07-19 06:38:18.592129+00	2026-07-12 06:38:18.592129+00	\N	f
cecf2aeb-96b7-43ed-b997-6635aeda1243	qdRdpU/rSfD8AsDe3jh4K2KvGUtQ4lHau0DN+QjNN5xuru+F1XUatbd9yTju6A8rIuT/sUjm4XMCr0CXjYawlA==	192.168.32.9	6	2026-07-19 06:47:06.383967+00	2026-07-12 06:47:06.383967+00	\N	f
6c5c30c3-22f2-43ec-a768-575c2c4374f4	fFyJeVU2EO5+OcVA0OaTtxbWVuioN9FGlyUyLeNyMG439/Uhb3oUVWLQJpf4aXHqWgAbzLLwNOhYEGuu7JEOAw==	192.168.32.9	3	2026-07-19 06:48:34.619305+00	2026-07-12 06:48:34.619307+00	\N	f
057e0794-b67b-4178-905f-0972f7e70f78	TZ5DLksJzxIxeLDXo9lGUmHfF/swoOTIbXyVPxFerya6COcWttgcL2CZnvxHoybbwr7F4/Zdj4lLSmZU5uxKaQ==	192.168.32.9	6	2026-07-19 06:48:48.472012+00	2026-07-12 06:48:48.472012+00	\N	f
685943db-1c0a-4f55-9007-efd708995e3f	hkFxMCbNxaExtejTo6xeuhjCM779so6ZWvNOEBgnRtCgpB3Q86bdG+lfCIL3XeCzeynPoWWjj4k9vCPsc5PVDw==	192.168.32.9	4	2026-07-19 06:49:04.956589+00	2026-07-12 06:49:04.956595+00	\N	f
c6011083-d154-4eb0-b9cd-32c4b4f1c473	W3gNjUdav6Me6IpH7zdI4TjCSRHSne2uDusN/CxmSmdzlg2j5+8/GUwyBEj++SCZQ4OPOjoRdGK5TOcwO0gqQw==	192.168.32.9	6	2026-07-19 06:53:59.507419+00	2026-07-12 06:53:59.507419+00	\N	f
d769e053-dc48-4d61-99bf-ba360939d238	OqmPH2IZ1mPkn0zmVpznLIT5jxiljgYCCTq6DUeoq8UhluW+0Vwwf3bsNp/ae8WVuyMn3pKm/T1a6OLYXJamig==	192.168.32.9	4	2026-07-19 09:26:12.080324+00	2026-07-12 09:26:12.080326+00	\N	f
87b7031e-7aed-454b-938b-84b006a271e6	QElsqXQSPItIK0ytEe4GrDBOlCHRj6y6SWjoHJ8h6l+a3CyW/v5eEpVDhyAXNKjhMongvrzeNU6xx0d8/uxqEQ==	192.168.32.9	6	2026-07-19 09:53:50.587748+00	2026-07-12 09:53:50.587755+00	\N	f
b7602e86-dda5-4620-a500-0293309815ef	jwRJZeES5X8Np9L+qVSdjJ3nCE7/vqmY5Aw3sw8T70r2YJwvSbXZ0QYjJg5ObMrZlS4kk4rBvmsW5AStVij1mg==	192.168.32.9	4	2026-07-19 10:20:40.326579+00	2026-07-12 10:20:40.326579+00	\N	f
\.


--
-- Data for Name: RoleClaims; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."RoleClaims" ("Id", "RoleId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: Roles; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Roles" ("Id", "Name", "NormalizedName", "ConcurrencyStamp") FROM stdin;
1	Admin	ADMIN	\N
2	Teacher	TEACHER	\N
3	Student	STUDENT	\N
\.


--
-- Data for Name: Screens; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Screens" ("Id", "Code", "Name", "Order", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: UserClaims; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."UserClaims" ("Id", "UserId", "ClaimType", "ClaimValue") FROM stdin;
\.


--
-- Data for Name: UserLogins; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."UserLogins" ("LoginProvider", "ProviderKey", "ProviderDisplayName", "UserId") FROM stdin;
Google	taiphanvan2403@gmail.com	Google	2
Google	legalassistant.dut@gmail.com	Google	3
Google	anhemdt3@gmail.com	Google	4
Google	mikechihao@gmail.com	Google	5
Email	pvt99x@gmail.com	Email	6
\.


--
-- Data for Name: UserRoles; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."UserRoles" ("UserId", "RoleId") FROM stdin;
1	1
2	3
3	2
4	3
5	3
6	2
\.


--
-- Data for Name: UserTokens; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."UserTokens" ("UserId", "LoginProvider", "Name", "Value") FROM stdin;
\.


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Users" ("Id", "FirstName", "LastName", "AvatarURL", "Phone", "Gender", "IsOnline", "IsActive", "CreatedAt", "UserName", "NormalizedUserName", "Email", "NormalizedEmail", "EmailConfirmed", "PasswordHash", "SecurityStamp", "ConcurrencyStamp", "PhoneNumber", "PhoneNumberConfirmed", "TwoFactorEnabled", "LockoutEnd", "LockoutEnabled", "AccessFailedCount") FROM stdin;
3	Legal Assistant DUT		https://lh3.googleusercontent.com/a/ACg8ocIFO4vz_dXERtA6OPBm_ZPtIELvcfdmyFJ7NXYVuyu--xjchMM=s96-c	\N	0	f	t	2026-07-07 08:47:56.758754+00	legalassistant.dut	LEGALASSISTANT.DUT	legalassistant.dut@gmail.com	LEGALASSISTANT.DUT@GMAIL.COM	t	AQAAAAIAAYagAAAAEO1GbBSS+6nWOWO37DcZ/FbmKONf+rGpK7Dbul1HxfmGlDagH7uhy9n6jf/GyI4qtQ==	35WTKBWSORMEWVJXJQ3S5FGVGVXYTW46	3fad19f9-c86d-4f1f-abec-8b55574b7e52	\N	f	f	\N	t	0
1	Admin	User	\N	\N	0	f	t	2026-07-07 07:47:08.283543+00	smartedu_admin	SMARTEDU_ADMIN	teampblpro@gmail.com	TEAMPBLPRO@GMAIL.COM	t	AQAAAAIAAYagAAAAEMmd/6aq+yzUHpcWZC6t9ADGDXWNzTmhiVBxlpGaL8Y7ZsEW0sHP0BgVpFBQvt5pOg==	ONZX3FIMXGXAYAZCIPJDPHXUYHOWR7DO	639228e0-242c-424c-a879-be387328ba13	\N	f	f	\N	t	0
4	Lập trình Web	Tự học	https://lh3.googleusercontent.com/a/ACg8ocJTfgaHAgnjL-HvlyrWKxgnX1WoBnoK-DQldLN59it2A1O18_Q=s96-c	\N	0	f	t	2026-07-07 09:52:02.588061+00	anhemdt3	ANHEMDT3	anhemdt3@gmail.com	ANHEMDT3@GMAIL.COM	t	AQAAAAIAAYagAAAAEKpyJrv2Qr21XpjyugOZbYF0obXk/EKJQ/B061zzq5peOQ5Gk2sW4Mj64BWadEdp8Q==	ASSACBHBMARRYVQ5PV7VTWLZJLFSXDP2	9f26e692-3790-42ef-aa70-9eaddc1b6b59	\N	f	f	\N	t	0
5	Mike Chi Hao		https://lh3.googleusercontent.com/a/ACg8ocLImiFUVeC32fWZi3n2SM2qYN92VGg7rpwHb5_x1--3n1EE=s96-c	\N	0	f	t	2026-07-07 10:00:19.170026+00	mikechihao	MIKECHIHAO	mikechihao@gmail.com	MIKECHIHAO@GMAIL.COM	t	AQAAAAIAAYagAAAAENySZdTBeQvj/3dgBqp+EfE1rRrTVQdxOME56MMDI12vv1TI3ocERnWaSJm0YkC7lg==	MRMD3HNY7QPJMTKSRXNUCOLI27FNEEP2	766bc77d-1014-4cdb-8003-1d626ba427b3	\N	f	f	\N	t	0
6			https://api.dicebear.com/9.x/miniavs/svg?seed=pvt99x	\N	0	f	t	2026-07-07 10:07:24.953746+00	pvt99x	PVT99X	pvt99x@gmail.com	PVT99X@GMAIL.COM	t	AQAAAAIAAYagAAAAEDhffAr/9bcnBH4dr/L3aR/5Iv3RGzeWgVE3WjA+gPU8qRll8QI5tsx7Kqr+UO9xOg==	CYXN3VD4P7FNVQ6L7DX5GF63Y5F7RAMG	13e1b2d3-3356-4ddd-b22a-02d46389774b	\N	f	f	\N	t	0
2	Tài	Phan Văn	https://lh3.googleusercontent.com/a/ACg8ocJsGxzIHOhPicxANjrM8yEoH8-VY6KMVRH83FGK1fPOaIVSpvfo=s96-c	\N	0	f	t	2026-07-07 08:46:07.817192+00	taiphanvan2403	TAIPHANVAN2403	taiphanvan2403@gmail.com	TAIPHANVAN2403@GMAIL.COM	t	AQAAAAIAAYagAAAAEGSXVx20ZrNYfYRhzPnayEdb+eBOfl6uIkBgH9y3/nLw5Q8HnwMXSeSS0dZZpuELDA==	QSFWQAXZSYSLA2RYLLIMEH6O525ZYXH3	c5279404-a123-45d0-88f2-0c9de6a6e1f2	\N	f	f	\N	t	0
\.


--
-- Name: RoleClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."RoleClaims_Id_seq"', 1, false);


--
-- Name: Roles_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."Roles_Id_seq"', 3, true);


--
-- Name: UserClaims_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."UserClaims_Id_seq"', 1, false);


--
-- Name: Users_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."Users_Id_seq"', 6, true);


--
-- Name: CoursePermissions PK_CoursePermissions; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CoursePermissions"
    ADD CONSTRAINT "PK_CoursePermissions" PRIMARY KEY ("CourseId", "AssistantId", "FunctionId");


--
-- Name: Functions PK_Functions; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Functions"
    ADD CONSTRAINT "PK_Functions" PRIMARY KEY ("Id");


--
-- Name: Permissions PK_Permissions; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Permissions"
    ADD CONSTRAINT "PK_Permissions" PRIMARY KEY ("RoleId", "FunctionId");


--
-- Name: RefreshTokens PK_RefreshTokens; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT "PK_RefreshTokens" PRIMARY KEY ("Id");


--
-- Name: RoleClaims PK_RoleClaims; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."RoleClaims"
    ADD CONSTRAINT "PK_RoleClaims" PRIMARY KEY ("Id");


--
-- Name: Roles PK_Roles; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT "PK_Roles" PRIMARY KEY ("Id");


--
-- Name: Screens PK_Screens; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Screens"
    ADD CONSTRAINT "PK_Screens" PRIMARY KEY ("Id");


--
-- Name: UserClaims PK_UserClaims; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserClaims"
    ADD CONSTRAINT "PK_UserClaims" PRIMARY KEY ("Id");


--
-- Name: UserLogins PK_UserLogins; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserLogins"
    ADD CONSTRAINT "PK_UserLogins" PRIMARY KEY ("LoginProvider", "ProviderKey");


--
-- Name: UserRoles PK_UserRoles; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "PK_UserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: UserTokens PK_UserTokens; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserTokens"
    ADD CONSTRAINT "PK_UserTokens" PRIMARY KEY ("UserId", "LoginProvider", "Name");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("Id");


--
-- Name: EmailIndex; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "EmailIndex" ON public."Users" USING btree ("NormalizedEmail");


--
-- Name: IX_CoursePermissions_FunctionId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_CoursePermissions_FunctionId" ON public."CoursePermissions" USING btree ("FunctionId");


--
-- Name: IX_Functions_ScreenId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Functions_ScreenId" ON public."Functions" USING btree ("ScreenId");


--
-- Name: IX_Permissions_FunctionId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Permissions_FunctionId" ON public."Permissions" USING btree ("FunctionId");


--
-- Name: IX_RefreshTokens_Token_IsRevoked_Expires; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_RefreshTokens_Token_IsRevoked_Expires" ON public."RefreshTokens" USING btree ("Token", "IsRevoked", "Expires");


--
-- Name: IX_RefreshTokens_UserId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_RefreshTokens_UserId" ON public."RefreshTokens" USING btree ("UserId");


--
-- Name: IX_RoleClaims_RoleId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_RoleClaims_RoleId" ON public."RoleClaims" USING btree ("RoleId");


--
-- Name: IX_UserClaims_UserId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_UserClaims_UserId" ON public."UserClaims" USING btree ("UserId");


--
-- Name: IX_UserLogins_UserId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_UserLogins_UserId" ON public."UserLogins" USING btree ("UserId");


--
-- Name: IX_UserRoles_RoleId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_UserRoles_RoleId" ON public."UserRoles" USING btree ("RoleId");


--
-- Name: RoleNameIndex; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "RoleNameIndex" ON public."Roles" USING btree ("NormalizedName");


--
-- Name: UserNameIndex; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "UserNameIndex" ON public."Users" USING btree ("NormalizedUserName");


--
-- Name: CoursePermissions FK_CoursePermissions_Functions_FunctionId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CoursePermissions"
    ADD CONSTRAINT "FK_CoursePermissions_Functions_FunctionId" FOREIGN KEY ("FunctionId") REFERENCES public."Functions"("Id") ON DELETE RESTRICT;


--
-- Name: Functions FK_Functions_Screens_ScreenId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Functions"
    ADD CONSTRAINT "FK_Functions_Screens_ScreenId" FOREIGN KEY ("ScreenId") REFERENCES public."Screens"("Id") ON DELETE CASCADE;


--
-- Name: Permissions FK_Permissions_Functions_FunctionId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Permissions"
    ADD CONSTRAINT "FK_Permissions_Functions_FunctionId" FOREIGN KEY ("FunctionId") REFERENCES public."Functions"("Id") ON DELETE CASCADE;


--
-- Name: RefreshTokens FK_RefreshTokens_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."RefreshTokens"
    ADD CONSTRAINT "FK_RefreshTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: RoleClaims FK_RoleClaims_Roles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."RoleClaims"
    ADD CONSTRAINT "FK_RoleClaims_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."Roles"("Id") ON DELETE CASCADE;


--
-- Name: UserClaims FK_UserClaims_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserClaims"
    ADD CONSTRAINT "FK_UserClaims_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: UserLogins FK_UserLogins_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserLogins"
    ADD CONSTRAINT "FK_UserLogins_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: UserRoles FK_UserRoles_Roles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."Roles"("Id") ON DELETE CASCADE;


--
-- Name: UserRoles FK_UserRoles_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: UserTokens FK_UserTokens_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserTokens"
    ADD CONSTRAINT "FK_UserTokens_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict RUR7F6Ofwap8COTfbLu7uVGQMiiyDl5PxcTNX5OhnY8c64ifAfWPu0QkisYaxuI

--
-- Database "EduSmart.CourseManagementService" dump
--

--
-- PostgreSQL database dump
--

\restrict 3NQubuZQTFpgeajCkhdYhlOoMP2fzsI1RAiWzbPmaxAEqSSgMzQKlEdDehLwqBQ

-- Dumped from database version 17.10 (Debian 17.10-1.pgdg13+1)
-- Dumped by pg_dump version 17.10 (Debian 17.10-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: EduSmart.CourseManagementService; Type: DATABASE; Schema: -; Owner: pbl6duter
--

CREATE DATABASE "EduSmart.CourseManagementService" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE "EduSmart.CourseManagementService" OWNER TO pbl6duter;

\unrestrict 3NQubuZQTFpgeajCkhdYhlOoMP2fzsI1RAiWzbPmaxAEqSSgMzQKlEdDehLwqBQ
\connect "EduSmart.CourseManagementService"
\restrict 3NQubuZQTFpgeajCkhdYhlOoMP2fzsI1RAiWzbPmaxAEqSSgMzQKlEdDehLwqBQ

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Bookmarks; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Bookmarks" (
    "Id" uuid NOT NULL,
    "UserId" integer NOT NULL,
    "LessonId" uuid NOT NULL,
    "CourseId" uuid NOT NULL
);


ALTER TABLE public."Bookmarks" OWNER TO pbl6duter;

--
-- Name: Categories; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Categories" (
    "Id" integer NOT NULL,
    "Name" character varying(100) NOT NULL,
    "CreatedBy" integer NOT NULL,
    "IsCreatedByAdmin" boolean NOT NULL,
    "WebIconInfo" json,
    "MobileIconInfo" json,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Categories" OWNER TO pbl6duter;

--
-- Name: Categories_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."Categories" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Categories_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Chapters; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Chapters" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Order" integer NOT NULL,
    "IsPublished" boolean DEFAULT true NOT NULL,
    "CourseId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Chapters" OWNER TO pbl6duter;

--
-- Name: Comments; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Comments" (
    "Id" uuid NOT NULL,
    "Content" text,
    "CreatedBy" integer NOT NULL,
    "MentionedUserId" integer,
    "VotersCount" integer NOT NULL,
    "ParentId" uuid,
    "IsApproved" boolean NOT NULL,
    "DiscussionId" uuid NOT NULL,
    "RoleOfUser" text NOT NULL,
    "IsDelFlag" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Comments" OWNER TO pbl6duter;

--
-- Name: COLUMN "Comments"."RoleOfUser"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Comments"."RoleOfUser" IS 'Role of the user who created the comment';


--
-- Name: CourseEnrollments; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."CourseEnrollments" (
    "StudentId" integer NOT NULL,
    "CourseId" uuid NOT NULL,
    "EnrollmentDate" timestamp with time zone NOT NULL,
    "CompletionDate" timestamp with time zone,
    "LeaveDate" timestamp with time zone,
    "IsCompleted" boolean NOT NULL,
    "VisibilityStatus" character varying(50) DEFAULT 'Unknown'::character varying NOT NULL
);


ALTER TABLE public."CourseEnrollments" OWNER TO pbl6duter;

--
-- Name: COLUMN "CourseEnrollments"."VisibilityStatus"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."CourseEnrollments"."VisibilityStatus" IS 'Allow other students to see the progress of this student in this course';


--
-- Name: CourseRatings; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."CourseRatings" (
    "Id" uuid NOT NULL,
    "CourseId" uuid NOT NULL,
    "UserId" bigint NOT NULL,
    "Rating" double precision NOT NULL,
    "Comment" character varying(200),
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."CourseRatings" OWNER TO pbl6duter;

--
-- Name: COLUMN "CourseRatings"."UserId"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."CourseRatings"."UserId" IS 'This user maybe a student or a teacher';


--
-- Name: CourseTags; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."CourseTags" (
    "Id" integer NOT NULL,
    "CourseId" uuid NOT NULL,
    "TagId" integer NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."CourseTags" OWNER TO pbl6duter;

--
-- Name: CourseTags_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."CourseTags" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."CourseTags_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Courses; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Courses" (
    "Id" uuid NOT NULL,
    "Name" character varying(200) NOT NULL,
    "Description" text,
    "CoreValues" json,
    "Prerequisites" json,
    "ThumbnailURL" text,
    "PreviewVideoURL" character varying(300),
    "Price" numeric NOT NULL,
    "CurrencyId" integer NOT NULL,
    "Type" character varying(50) NOT NULL,
    "TeacherId" integer NOT NULL,
    "IsPublished" boolean DEFAULT true NOT NULL,
    "CategoryId" integer NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Courses" OWNER TO pbl6duter;

--
-- Name: Currencies; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Currencies" (
    "Id" integer NOT NULL,
    "Name" character varying(100) NOT NULL,
    "Code" character varying(3) NOT NULL
);


ALTER TABLE public."Currencies" OWNER TO pbl6duter;

--
-- Name: Currencies_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."Currencies" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Currencies_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: DiscussionTypes; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."DiscussionTypes" (
    "Id" integer NOT NULL,
    "Name" character varying(255) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."DiscussionTypes" OWNER TO pbl6duter;

--
-- Name: DiscussionTypes_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."DiscussionTypes" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."DiscussionTypes_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: Discussions; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Discussions" (
    "Id" uuid NOT NULL,
    "Title" character varying(255) NOT NULL,
    "Content" text NOT NULL,
    "IsAnswered" boolean NOT NULL,
    "DeletedAt" timestamp with time zone,
    "IsDelFlag" boolean NOT NULL,
    "CreatedBy" integer NOT NULL,
    "RoleOfUser" text NOT NULL,
    "TypeId" integer NOT NULL,
    "LessonId" uuid NOT NULL,
    "CourseId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Discussions" OWNER TO pbl6duter;

--
-- Name: COLUMN "Discussions"."RoleOfUser"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Discussions"."RoleOfUser" IS 'Role of the user who created the discussion';


--
-- Name: COLUMN "Discussions"."TypeId"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Discussions"."TypeId" IS 'Id of the discussion type';


--
-- Name: LessonRatings; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."LessonRatings" (
    "Id" uuid NOT NULL,
    "LessonId" uuid NOT NULL,
    "StudentId" bigint NOT NULL,
    "IsLike" boolean NOT NULL,
    "Comment" character varying(200),
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."LessonRatings" OWNER TO pbl6duter;

--
-- Name: LessonTrackings; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."LessonTrackings" (
    "Id" uuid NOT NULL,
    "LessonId" uuid NOT NULL,
    "CourseId" uuid NOT NULL,
    "StudentId" bigint NOT NULL,
    "TimeSpent" integer NOT NULL,
    "IsCompleted" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."LessonTrackings" OWNER TO pbl6duter;

--
-- Name: COLUMN "LessonTrackings"."TimeSpent"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."LessonTrackings"."TimeSpent" IS 'The time spent on the lesson in seconds';


--
-- Name: Lessons; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Lessons" (
    "Id" uuid NOT NULL,
    "Title" character varying(200) NOT NULL,
    "Description" text,
    "ChapterId" uuid NOT NULL,
    "DurationInSeconds" bigint NOT NULL,
    "LessonType" character varying(50) NOT NULL,
    "CreatedBy" bigint NOT NULL,
    "IsPublished" boolean NOT NULL,
    "PublishedAt" timestamp with time zone,
    "IsCommentAllowed" boolean DEFAULT true NOT NULL,
    "IsRatingAllowed" boolean DEFAULT true NOT NULL,
    "Order" integer NOT NULL,
    "DifficultyLevel" character varying(50) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Lessons" OWNER TO pbl6duter;

--
-- Name: COLUMN "Lessons"."Description"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Lessons"."Description" IS 'The text content of the lesson';


--
-- Name: COLUMN "Lessons"."IsPublished"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Lessons"."IsPublished" IS 'Maybe the lesson is not ready to be published';


--
-- Name: Notes; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Notes" (
    "Id" uuid NOT NULL,
    "Content" text NOT NULL,
    "Comment" text,
    "UserId" integer NOT NULL,
    "TimeMilestone" integer NOT NULL,
    "LessonId" uuid NOT NULL,
    "ChapterId" uuid NOT NULL,
    "LessonType" character varying(50) NOT NULL,
    "IsDelFlag" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Notes" OWNER TO pbl6duter;

--
-- Name: COLUMN "Notes"."Content"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Notes"."Content" IS 'Highlighted text from the lesson';


--
-- Name: COLUMN "Notes"."Comment"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Notes"."Comment" IS 'Note about the highlighted text';


--
-- Name: COLUMN "Notes"."TimeMilestone"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Notes"."TimeMilestone" IS 'The time of the lesson when the note was created';


--
-- Name: NotificationBellClickLogs; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."NotificationBellClickLogs" (
    "Id" uuid NOT NULL,
    "UserId" integer NOT NULL,
    "LastClickedAt" timestamp with time zone NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."NotificationBellClickLogs" OWNER TO pbl6duter;

--
-- Name: QuizAnswers; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."QuizAnswers" (
    "Id" bigint NOT NULL,
    "Answer" character varying(500) NOT NULL,
    "IsCorrect" boolean NOT NULL,
    "Explanation" character varying(500),
    "QuizLessonId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."QuizAnswers" OWNER TO pbl6duter;

--
-- Name: COLUMN "QuizAnswers"."Explanation"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."QuizAnswers"."Explanation" IS 'Explanation why the answer is correct or incorrect';


--
-- Name: QuizAnswers_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."QuizAnswers" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."QuizAnswers_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: QuizLessons; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."QuizLessons" (
    "Id" uuid NOT NULL,
    "Question" text NOT NULL,
    "LessonId" uuid NOT NULL,
    "IsMultipleChoice" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."QuizLessons" OWNER TO pbl6duter;

--
-- Name: Reactions; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Reactions" (
    "Id" uuid NOT NULL,
    "UserId" integer NOT NULL,
    "Type" character varying(50) NOT NULL,
    "CommentId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."Reactions" OWNER TO pbl6duter;

--
-- Name: SupportRequests; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."SupportRequests" (
    "Id" uuid NOT NULL,
    "FromUserId" integer NOT NULL,
    "ToUserId" integer NOT NULL,
    "Description" text,
    "Response" text,
    "ResolvedAt" timestamp with time zone,
    "Status" character varying(50) NOT NULL,
    "Type" character varying(100) NOT NULL,
    "IsPublic" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."SupportRequests" OWNER TO pbl6duter;

--
-- Name: COLUMN "SupportRequests"."FromUserId"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."SupportRequests"."FromUserId" IS 'UserId proceed the request';


--
-- Name: COLUMN "SupportRequests"."ToUserId"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."SupportRequests"."ToUserId" IS 'UserId to proceed the request, such as TeacherId, AdminId';


--
-- Name: Tags; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Tags" (
    "Id" integer NOT NULL,
    "Name" character varying(100) NOT NULL,
    "CreatedBy" integer NOT NULL,
    "IsCreatedByAdmin" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."Tags" OWNER TO pbl6duter;

--
-- Name: Tags_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."Tags" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."Tags_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: UserNotifications; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."UserNotifications" (
    "Id" uuid NOT NULL,
    "ReceiverId" integer NOT NULL,
    "SenderInfo" jsonb,
    "Type" character varying(100) NOT NULL,
    "RelatedEntityId" character varying(36) NOT NULL,
    "RelatedEntityType" character varying(50) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "CourseId" uuid NOT NULL,
    "IsRead" boolean NOT NULL,
    "MetaData" character varying(2000) NOT NULL
);


ALTER TABLE public."UserNotifications" OWNER TO pbl6duter;

--
-- Name: VideoLessons; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."VideoLessons" (
    "Id" uuid NOT NULL,
    "BaseBlobURL" text NOT NULL,
    "ThumbnailURL" character varying(200),
    "StorageSize" bigint NOT NULL,
    "UploadStatus" character varying(50) NOT NULL,
    "LessonId" uuid NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone
);


ALTER TABLE public."VideoLessons" OWNER TO pbl6duter;

--
-- Name: COLUMN "VideoLessons"."BaseBlobURL"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."VideoLessons"."BaseBlobURL" IS 'URL without SAS token';


--
-- Data for Name: Bookmarks; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Bookmarks" ("Id", "UserId", "LessonId", "CourseId") FROM stdin;
\.


--
-- Data for Name: Categories; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Categories" ("Id", "Name", "CreatedBy", "IsCreatedByAdmin", "WebIconInfo", "MobileIconInfo", "CreatedAt", "UpdatedAt") FROM stdin;
1	Programming	1	t	{"Icon":"FaCode","Color":"#E83E8C"}	{"Icon":"0xf653","Color":""}	2026-07-07 07:47:08.331887+00	\N
2	Music	1	t	{"Icon":"FaMusic","Color":"#FF6347"}	{"Icon":"0xf1fb","Color":""}	2026-07-07 07:47:08.331887+00	\N
3	Language	1	t	{"Icon":"FaLanguage","Color":"#1E90FF"}	{"Icon":"0xf45e","Color":""}	2026-07-07 07:47:08.331887+00	\N
4	Design	1	t	{"Icon":"FaRuler","Color":"#FFD700"}	{"Icon":"0xefaf","Color":""}	2026-07-07 07:47:08.331887+00	\N
5	Marketing	1	t	{"Icon":"FaChartLine","Color":"#32CD32"}	{"Icon":"0xef0a","Color":""}	2026-07-07 07:47:08.331887+00	\N
\.


--
-- Data for Name: Chapters; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Chapters" ("Id", "Name", "Order", "IsPublished", "CourseId", "CreatedAt", "UpdatedAt") FROM stdin;
5ced02be-4367-478e-b7e4-25c6be056363	Bắt đầu	1	t	1ee082d2-24b4-425d-9af7-56196fc58460	2026-07-07 09:53:42.395118+00	\N
78b67b36-a8f7-4df2-af0c-a00b91e65b8c	Làm quen với HTML	2	t	1ee082d2-24b4-425d-9af7-56196fc58460	2026-07-07 09:54:02.030462+00	2026-07-07 09:54:28.923551+00
36877b90-a4d8-41ae-9b3c-0ae5367275a1	Các thẻ tiêu đề	3	t	1ee082d2-24b4-425d-9af7-56196fc58460	2026-07-07 09:54:15.927484+00	2026-07-07 09:54:34.284862+00
c567c523-cf05-47ef-8fbe-a726a29bf553	Thẻ đoạn văn	4	t	1ee082d2-24b4-425d-9af7-56196fc58460	2026-07-07 09:59:56.070391+00	\N
d4af8e77-56dd-4882-97f6-74c43ebe5cf9	Chữ đậm, chữ nghiêng	5	t	1ee082d2-24b4-425d-9af7-56196fc58460	2026-07-07 10:00:09.616935+00	2026-07-07 10:02:47.605574+00
492fd04d-7674-4f27-980c-838b97f880c0	Làm quen với HTML	2	t	518d86a9-1939-4aa1-ae8c-d5e695852170	2026-07-07 15:59:18.994661+00	2026-07-07 15:59:26.31866+00
43a09210-bd2e-4526-8ef7-d2e42b1bc2e3	Mở đầu	1	t	518d86a9-1939-4aa1-ae8c-d5e695852170	2026-07-07 15:41:04.387676+00	2026-07-07 15:59:31.141491+00
\.


--
-- Data for Name: Comments; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Comments" ("Id", "Content", "CreatedBy", "MentionedUserId", "VotersCount", "ParentId", "IsApproved", "DiscussionId", "RoleOfUser", "IsDelFlag", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: CourseEnrollments; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."CourseEnrollments" ("StudentId", "CourseId", "EnrollmentDate", "CompletionDate", "LeaveDate", "IsCompleted", "VisibilityStatus") FROM stdin;
\.


--
-- Data for Name: CourseRatings; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."CourseRatings" ("Id", "CourseId", "UserId", "Rating", "Comment", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: CourseTags; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."CourseTags" ("Id", "CourseId", "TagId", "CreatedAt", "UpdatedAt") FROM stdin;
1	1ee082d2-24b4-425d-9af7-56196fc58460	7	2026-07-07 09:04:30.734943+00	\N
\.


--
-- Data for Name: Courses; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Courses" ("Id", "Name", "Description", "CoreValues", "Prerequisites", "ThumbnailURL", "PreviewVideoURL", "Price", "CurrencyId", "Type", "TeacherId", "IsPublished", "CategoryId", "CreatedAt", "UpdatedAt") FROM stdin;
1ee082d2-24b4-425d-9af7-56196fc58460	HTML, CSS cơ bản	\N	["HTML","CSS","Web Design"]	[]	https://localhost:9000/digitization-dev/ec4d7aec-a1ca-4f57-a984-baed805ad42d	https://localhost:9000/digitization-dev/1ee082d2-24b4-425d-9af7-56196fc58460	5000	1	Tutorial	3	f	1	2026-07-07 09:04:30.734943+00	2026-07-07 09:04:33.334154+00
518d86a9-1939-4aa1-ae8c-d5e695852170	Lập trình WEB	\N	["HTML"]	[]	https://localhost:9000/digitization-dev/9e9ce758-25ec-4dec-aa98-c83b9fbe9d69	https://localhost:9000/digitization-dev/518d86a9-1939-4aa1-ae8c-d5e695852170	2000	1	Tutorial	6	f	1	2026-07-07 10:32:43.487529+00	2026-07-07 10:32:45.3821+00
\.


--
-- Data for Name: Currencies; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Currencies" ("Id", "Name", "Code") FROM stdin;
1	Viet Nam Dong	VND
2	US Dollar	USD
\.


--
-- Data for Name: DiscussionTypes; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."DiscussionTypes" ("Id", "Name", "CreatedAt", "UpdatedAt") FROM stdin;
1	Show dự án	2026-07-07 07:47:08.331887+00	\N
2	Báo lỗi bài học	2026-07-07 07:47:08.331887+00	\N
3	Cần trợ giúp	2026-07-07 07:47:08.331887+00	\N
4	Đóng góp ý kiến	2026-07-07 07:47:08.331887+00	\N
5	Khác	2026-07-07 07:47:08.331887+00	\N
\.


--
-- Data for Name: Discussions; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Discussions" ("Id", "Title", "Content", "IsAnswered", "DeletedAt", "IsDelFlag", "CreatedBy", "RoleOfUser", "TypeId", "LessonId", "CourseId", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: LessonRatings; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."LessonRatings" ("Id", "LessonId", "StudentId", "IsLike", "Comment", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: LessonTrackings; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."LessonTrackings" ("Id", "LessonId", "CourseId", "StudentId", "TimeSpent", "IsCompleted", "CreatedAt", "UpdatedAt") FROM stdin;
89ba7072-81b1-4575-8214-345f49bb7152	4785ced3-88ca-4f94-bb7d-0bdd4973aa84	518d86a9-1939-4aa1-ae8c-d5e695852170	6	0	t	2026-07-07 17:18:10.819818+00	\N
9fd9fc33-7ead-4ef6-8be8-ad00edc24e04	371aeeee-6b1c-4826-bae1-300af3ee8183	518d86a9-1939-4aa1-ae8c-d5e695852170	6	548	t	2026-07-07 17:18:10.819818+00	2026-07-07 17:19:06.227612+00
a2923e4d-6417-4f03-bc89-3f4345e500ab	21da97c9-a17d-468c-aff2-5c03bcd7cebf	518d86a9-1939-4aa1-ae8c-d5e695852170	6	0	t	2026-07-07 17:19:02.743807+00	2026-07-07 17:19:15.172252+00
1bc57a07-eb8e-4fd5-a35b-5d181f8279b3	e6ae43af-a041-4e77-acbf-86a83c0cfa93	518d86a9-1939-4aa1-ae8c-d5e695852170	6	0	f	2026-07-07 17:19:15.173173+00	\N
\.


--
-- Data for Name: Lessons; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Lessons" ("Id", "Title", "Description", "ChapterId", "DurationInSeconds", "LessonType", "CreatedBy", "IsPublished", "PublishedAt", "IsCommentAllowed", "IsRatingAllowed", "Order", "DifficultyLevel", "CreatedAt", "UpdatedAt") FROM stdin;
21da97c9-a17d-468c-aff2-5c03bcd7cebf	Ngôn ngữ trình duyệt có thể hiểu?	\N	492fd04d-7674-4f27-980c-838b97f880c0	0	Quiz	6	t	2026-07-07 16:01:15.960265+00	t	t	1	Easy	2026-07-07 16:01:15.982222+00	\N
371aeeee-6b1c-4826-bae1-300af3ee8183	1.3 Kinh nghiệm học hiệu quả	<p id="isPasted" fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"># Cấu h&igrave;nh m&aacute;y t&iacute;nh ph&ugrave; hợp để học Web</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Để học v&agrave; l&agrave;m việc trong lĩnh vực **Web Development**, bạn **kh&ocirc;ng cần** một m&aacute;y t&iacute;nh c&oacute; cấu h&igrave;nh qu&aacute; cao. Chỉ với một cấu h&igrave;nh tầm trung l&agrave; đ&atilde; đ&aacute;p ứng tốt nhu cầu học tập v&agrave; lập tr&igrave;nh.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">## Cấu h&igrave;nh khuyến nghị</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Th&agrave;nh phần | Khuyến nghị |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">|------------|-------------|</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| CPU | Intel Core i3 hoặc i5 (hoặc tương đương) |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| RAM | Từ **8 GB** trở l&ecirc;n |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Ổ cứng | SSD, dung lượng từ **80 GB** trở l&ecirc;n |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| M&agrave;n h&igrave;nh | Từ **14 inch**, phổ biến l&agrave; **15.6 inch** |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Độ ph&acirc;n giải | Từ **1366 &times; 768**, khuyến nghị **1920 &times; 1080 (Full HD)** |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Card đồ họa | Kh&ocirc;ng bắt buộc c&oacute; GPU rời |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Hệ điều h&agrave;nh | Windows 10 hoặc Windows 11 |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">&gt; Kh&ocirc;ng cần c&agrave;i đặt th&ecirc;m phần mềm đặc biệt trước khi bắt đầu kh&oacute;a học.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">---</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">## Nếu sử dụng Ubuntu hoặc macOS</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Bạn ho&agrave;n to&agrave;n c&oacute; thể học bằng **Ubuntu** hoặc **macOS**.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Trong một số b&agrave;i hướng dẫn, thao t&aacute;c tr&ecirc;n Windows c&oacute; thể kh&aacute;c đ&ocirc;i ch&uacute;t so với Ubuntu hoặc macOS. Tuy nhi&ecirc;n, sự kh&aacute;c biệt n&agrave;y thường kh&ocirc;ng g&acirc;y nhiều kh&oacute; khăn.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Thực tế, nhiều người sử dụng Ubuntu hoặc macOS đều đ&atilde; quen với Windows trước đ&oacute;, n&ecirc;n việc chuyển đổi giữa c&aacute;c hệ điều h&agrave;nh kh&aacute; dễ d&agrave;ng.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">---</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"># C&oacute; n&ecirc;n sử dụng hai m&agrave;n h&igrave;nh?</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Việc sử dụng **hai m&agrave;n h&igrave;nh** kh&ocirc;ng phải l&agrave; y&ecirc;u cầu bắt buộc, nhưng nếu c&oacute; điều kiện th&igrave; đ&acirc;y l&agrave; một khoản đầu tư rất đ&aacute;ng gi&aacute;.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">### Lợi &iacute;ch của hai m&agrave;n h&igrave;nh</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Xem video b&agrave;i giảng tr&ecirc;n một m&agrave;n h&igrave;nh v&agrave; viết code tr&ecirc;n m&agrave;n h&igrave;nh c&ograve;n lại.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Hiển thị kết quả chạy chương tr&igrave;nh ở một m&agrave;n h&igrave;nh, chỉnh sửa m&atilde; nguồn ở m&agrave;n h&igrave;nh kia.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- C&oacute; nhiều kh&ocirc;ng gian l&agrave;m việc hơn.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Giảm số lần phải chuyển đổi giữa c&aacute;c cửa sổ hoặc tab.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">---</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"># C&oacute; n&ecirc;n sử dụng giấy nhớ v&agrave; b&uacute;t?</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Đối với kh&oacute;a học n&agrave;y, **kh&ocirc;ng bắt buộc** phải sử dụng giấy nhớ hoặc sổ ghi ch&eacute;p, v&igrave; nền tảng học tập đ&atilde; t&iacute;ch hợp sẵn c&ocirc;ng cụ ghi ch&uacute; trong từng b&agrave;i học.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Tuy nhi&ecirc;n, nếu bạn c&oacute; th&oacute;i quen ghi ch&eacute;p bằng tay th&igrave; đ&acirc;y vẫn l&agrave; một phương ph&aacute;p tốt.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">### Gợi &yacute;</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Chỉ ghi lại những kiến thức quan trọng hoặc kh&oacute; nhớ.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Viết t&ecirc;n c&aacute;c kh&aacute;i niệm, c&uacute; ph&aacute;p hoặc lưu &yacute; dễ qu&ecirc;n l&ecirc;n giấy nhớ.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- D&aacute;n giấy nhớ tại g&oacute;c học tập hoặc nơi l&agrave;m việc để thường xuy&ecirc;n nh&igrave;n thấy v&agrave; ghi nhớ.</p><p data-f-id="pbf" style="text-align: center; font-size: 14px; margin-top: 30px; opacity: 0.65; font-family: sans-serif;">Powered by <a href="https://www.froala.com/wysiwyg-editor?pb=1" title="Froala Editor">Froala Editor</a></p>	43a09210-bd2e-4526-8ef7-d2e42b1bc2e3	548	Video	6	t	2026-07-07 15:56:15.538835+00	t	t	2	Easy	2026-07-07 15:56:15.555016+00	2026-07-08 02:10:31.646937+00
e6ae43af-a041-4e77-acbf-86a83c0cfa93	Chi tiết về ngôn ngữ HTML	<p id="isPasted" fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"># Cấu h&igrave;nh m&aacute;y t&iacute;nh ph&ugrave; hợp để học Web</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Để học v&agrave; l&agrave;m việc trong lĩnh vực **Web Development**, bạn **kh&ocirc;ng cần** một m&aacute;y t&iacute;nh c&oacute; cấu h&igrave;nh qu&aacute; cao. Chỉ với một cấu h&igrave;nh tầm trung l&agrave; đ&atilde; đ&aacute;p ứng tốt nhu cầu học tập v&agrave; lập tr&igrave;nh.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">## Cấu h&igrave;nh khuyến nghị</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Th&agrave;nh phần | Khuyến nghị |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">|------------|-------------|</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| CPU | Intel Core i3 hoặc i5 (hoặc tương đương) |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| RAM | Từ **8 GB** trở l&ecirc;n |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Ổ cứng | SSD, dung lượng từ **80 GB** trở l&ecirc;n |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| M&agrave;n h&igrave;nh | Từ **14 inch**, phổ biến l&agrave; **15.6 inch** |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Độ ph&acirc;n giải | Từ **1366 &times; 768**, khuyến nghị **1920 &times; 1080 (Full HD)** |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Card đồ họa | Kh&ocirc;ng bắt buộc c&oacute; GPU rời |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">| Hệ điều h&agrave;nh | Windows 10 hoặc Windows 11 |</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">&gt; Kh&ocirc;ng cần c&agrave;i đặt th&ecirc;m phần mềm đặc biệt trước khi bắt đầu kh&oacute;a học.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">---</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">## Nếu sử dụng Ubuntu hoặc macOS</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Bạn ho&agrave;n to&agrave;n c&oacute; thể học bằng **Ubuntu** hoặc **macOS**.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Trong một số b&agrave;i hướng dẫn, thao t&aacute;c tr&ecirc;n Windows c&oacute; thể kh&aacute;c đ&ocirc;i ch&uacute;t so với Ubuntu hoặc macOS. Tuy nhi&ecirc;n, sự kh&aacute;c biệt n&agrave;y thường kh&ocirc;ng g&acirc;y nhiều kh&oacute; khăn.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Thực tế, nhiều người sử dụng Ubuntu hoặc macOS đều đ&atilde; quen với Windows trước đ&oacute;, n&ecirc;n việc chuyển đổi giữa c&aacute;c hệ điều h&agrave;nh kh&aacute; dễ d&agrave;ng.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">---</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"># C&oacute; n&ecirc;n sử dụng hai m&agrave;n h&igrave;nh?</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Việc sử dụng **hai m&agrave;n h&igrave;nh** kh&ocirc;ng phải l&agrave; y&ecirc;u cầu bắt buộc, nhưng nếu c&oacute; điều kiện th&igrave; đ&acirc;y l&agrave; một khoản đầu tư rất đ&aacute;ng gi&aacute;.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">### Lợi &iacute;ch của hai m&agrave;n h&igrave;nh</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Xem video b&agrave;i giảng tr&ecirc;n một m&agrave;n h&igrave;nh v&agrave; viết code tr&ecirc;n m&agrave;n h&igrave;nh c&ograve;n lại.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Hiển thị kết quả chạy chương tr&igrave;nh ở một m&agrave;n h&igrave;nh, chỉnh sửa m&atilde; nguồn ở m&agrave;n h&igrave;nh kia.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- C&oacute; nhiều kh&ocirc;ng gian l&agrave;m việc hơn.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Giảm số lần phải chuyển đổi giữa c&aacute;c cửa sổ hoặc tab.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">---</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"># C&oacute; n&ecirc;n sử dụng giấy nhớ v&agrave; b&uacute;t?</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Đối với kh&oacute;a học n&agrave;y, **kh&ocirc;ng bắt buộc** phải sử dụng giấy nhớ hoặc sổ ghi ch&eacute;p, v&igrave; nền tảng học tập đ&atilde; t&iacute;ch hợp sẵn c&ocirc;ng cụ ghi ch&uacute; trong từng b&agrave;i học.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Tuy nhi&ecirc;n, nếu bạn c&oacute; th&oacute;i quen ghi ch&eacute;p bằng tay th&igrave; đ&acirc;y vẫn l&agrave; một phương ph&aacute;p tốt.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">### Gợi &yacute;</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;"><br fr-original-style="" style="border: 0px solid rgb(229, 231, 235); box-sizing: border-box;"></p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Chỉ ghi lại những kiến thức quan trọng hoặc kh&oacute; nhớ.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- Viết t&ecirc;n c&aacute;c kh&aacute;i niệm, c&uacute; ph&aacute;p hoặc lưu &yacute; dễ qu&ecirc;n l&ecirc;n giấy nhớ.</p><p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">- D&aacute;n giấy nhớ tại g&oacute;c học tập hoặc nơi l&agrave;m việc để thường xuy&ecirc;n nh&igrave;n thấy v&agrave; ghi nhớ.</p><p data-f-id="pbf" style="text-align: center; font-size: 14px; margin-top: 30px; opacity: 0.65; font-family: sans-serif;">Powered by <a href="https://www.froala.com/wysiwyg-editor?pb=1" title="Froala Editor">Froala Editor</a></p>	492fd04d-7674-4f27-980c-838b97f880c0	406	Video	6	t	2026-07-07 16:05:44.507062+00	t	t	2	Easy	2026-07-07 16:05:44.507742+00	2026-07-08 02:11:13.854126+00
4785ced3-88ca-4f94-bb7d-0bdd4973aa84	1.2 Nội dung khóa học	# Thực hành làm dự án\nKhóa học này sẽ gồm hơn 50 chương học, các chương được sắp xếp theo các nhóm nội dung chính như sau:\n\nLàm quen với HTML, CSS cơ bản\nHọc cách dàn trang web\nThực hành làm dự án\n# Làm quen với HTML CSS\nTrong các chương đầu tiên bạn sẽ được tìm hiểu về các kiến thức sau:\n\n# Làm quen với HTML CSS\nHiểu HTML là gì, biết cách sử dụng các thẻ HTML thông dụng, hiểu ý nghĩa và trường hợp sử dụng của từng thẻ HTML.\n\nHiểu CSS là gì, biết cách sử dụng các thuộc tính của CSS, biết xử lý giao diện trang web cơ bản.\n\n# Làm việc với các loại nội dung\nLàm việc với văn bản và liên kết\nLàm việc với hình ảnh và video\nLàm việc với màu sắc và nền\nLàm việc với các loại nội dung khác\n# Dàn trang\nDàn trang là việc bạn làm ra một giao diện trang web dựa trên mẫu thiết kế cho trước. Mẫu thiết kế có thể đơn giản là một trang web khác (chúng ta làm lại giống như vậy), hay có thể là những thiết kế trên Figma.\n\nTrong phần này, các bạn sẽ học các kiến thức:\n\nCách phân biệt các thành phần trên giao diện trang web\nCách phân tích bố cục trang web từ tổng quan tới chi tiết\nCác triển khai từ phân tích tới code HTML CSS\nCách đặt tên class và tái sử dụng code\n# Thực hành làm dự án\nĐây là phần rất quan trọng giúp bạn có đầy đủ kỹ năng thực tế để có thể làm ra hầu hết các giao diện trang web phổ biến.\n\nChúng ta có tổng cộng 8 dự án trong khóa học này, mọi dự án đều có thiết kế Figma, trong đó có tới 3 dự án có thêm thiết kế giao diện dành cho di động.	43a09210-bd2e-4526-8ef7-d2e42b1bc2e3	0	Text	6	t	2026-07-07 15:44:06.326168+00	t	t	1	Easy	2026-07-07 15:44:06.346811+00	2026-07-12 09:17:19.566407+00
\.


--
-- Data for Name: Notes; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Notes" ("Id", "Content", "Comment", "UserId", "TimeMilestone", "LessonId", "ChapterId", "LessonType", "IsDelFlag", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: NotificationBellClickLogs; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."NotificationBellClickLogs" ("Id", "UserId", "LastClickedAt", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: QuizAnswers; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."QuizAnswers" ("Id", "Answer", "IsCorrect", "Explanation", "QuizLessonId", "CreatedAt", "UpdatedAt") FROM stdin;
1	Java, Python, PHP	f	Chưa chính xác! Java, Python và PHP là các ngôn ngữ lập trình chạy trên máy chủ, trình duyệt không thể chạy được chúng.	b2c5d538-3fbb-464c-93d3-3e1a5394bdca	2026-07-07 16:01:15.982222+00	\N
2	HTML, CSS, JS	t	Chính xác! HTML, CSS và JavaScript là 3 ngôn ngữ duy nhất mà trình duyệt có thể hiểu.	b2c5d538-3fbb-464c-93d3-3e1a5394bdca	2026-07-07 16:01:15.982222+00	\N
3	HTML, CSS, C++	f	Chưa chính xác! Trong câu trả lời này có C++ là ngôn ngữ mà trình duyệt không thể chạy được.	b2c5d538-3fbb-464c-93d3-3e1a5394bdca	2026-07-07 16:01:15.982222+00	\N
\.


--
-- Data for Name: QuizLessons; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."QuizLessons" ("Id", "Question", "LessonId", "IsMultipleChoice", "CreatedAt", "UpdatedAt") FROM stdin;
b2c5d538-3fbb-464c-93d3-3e1a5394bdca	<p fr-original-style="" style="border: 0px solid rgb(229, 231, 235); margin: 0px; box-sizing: border-box;">Ng&ocirc;n ngữ tr&igrave;nh duyệt c&oacute; thể hiểu?</p><p data-f-id="pbf" style="text-align: center; font-size: 14px; margin-top: 30px; opacity: 0.65; font-family: sans-serif;">Powered by <a href="https://www.froala.com/wysiwyg-editor?pb=1" title="Froala Editor">Froala Editor</a></p>	21da97c9-a17d-468c-aff2-5c03bcd7cebf	f	2026-07-07 16:01:15.982222+00	\N
\.


--
-- Data for Name: Reactions; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Reactions" ("Id", "UserId", "Type", "CommentId", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: SupportRequests; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."SupportRequests" ("Id", "FromUserId", "ToUserId", "Description", "Response", "ResolvedAt", "Status", "Type", "IsPublic", "CreatedAt", "UpdatedAt") FROM stdin;
\.


--
-- Data for Name: Tags; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Tags" ("Id", "Name", "CreatedBy", "IsCreatedByAdmin", "CreatedAt", "UpdatedAt") FROM stdin;
1	C/C++	0	t	2026-07-07 07:47:08.331887+00	\N
2	C#	0	t	2026-07-07 07:47:08.331887+00	\N
3	Java	0	t	2026-07-07 07:47:08.331887+00	\N
4	Python	0	t	2026-07-07 07:47:08.331887+00	\N
5	AI	0	t	2026-07-07 07:47:08.331887+00	\N
6	Cloud	0	t	2026-07-07 07:47:08.331887+00	\N
7	HTML, CSS	0	t	2026-07-07 07:47:08.331887+00	\N
8	Basic Javascript	0	t	2026-07-07 07:47:08.331887+00	\N
9	Toeic	0	t	2026-07-07 07:47:08.331887+00	\N
10	Listening	0	t	2026-07-07 07:47:08.331887+00	\N
11	Reading	0	t	2026-07-07 07:47:08.331887+00	\N
\.


--
-- Data for Name: UserNotifications; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."UserNotifications" ("Id", "ReceiverId", "SenderInfo", "Type", "RelatedEntityId", "RelatedEntityType", "CreatedAt", "CourseId", "IsRead", "MetaData") FROM stdin;
\.


--
-- Data for Name: VideoLessons; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."VideoLessons" ("Id", "BaseBlobURL", "ThumbnailURL", "StorageSize", "UploadStatus", "LessonId", "CreatedAt", "UpdatedAt") FROM stdin;
96619039-a4ee-48ef-88f7-35e21a8eee9d	https://localhost:9000/digitization-dev/371aeeee-6b1c-4826-bae1-300af3ee8183	https://res.cloudinary.com/da1aqhx1g/image/upload/f_auto,q_auto/v1/default-assets/rklr1cd3da0mkulzjq6b	0	Completed	371aeeee-6b1c-4826-bae1-300af3ee8183	2026-07-07 15:56:15.555016+00	2026-07-07 15:56:19.008167+00
dab4bbcf-dfff-4a5b-9f8c-e9acae9488b1	https://localhost:9000/digitization-dev/e6ae43af-a041-4e77-acbf-86a83c0cfa93	https://res.cloudinary.com/da1aqhx1g/image/upload/f_auto,q_auto/v1/default-assets/rklr1cd3da0mkulzjq6b	0	Completed	e6ae43af-a041-4e77-acbf-86a83c0cfa93	2026-07-07 16:05:44.507742+00	2026-07-07 16:05:46.813606+00
\.


--
-- Name: Categories_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."Categories_Id_seq"', 5, true);


--
-- Name: CourseTags_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."CourseTags_Id_seq"', 1, true);


--
-- Name: Currencies_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."Currencies_Id_seq"', 2, true);


--
-- Name: DiscussionTypes_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."DiscussionTypes_Id_seq"', 5, true);


--
-- Name: QuizAnswers_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."QuizAnswers_Id_seq"', 3, true);


--
-- Name: Tags_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."Tags_Id_seq"', 11, true);


--
-- Name: Bookmarks PK_Bookmarks; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Bookmarks"
    ADD CONSTRAINT "PK_Bookmarks" PRIMARY KEY ("Id");


--
-- Name: Categories PK_Categories; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Categories"
    ADD CONSTRAINT "PK_Categories" PRIMARY KEY ("Id");


--
-- Name: Chapters PK_Chapters; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Chapters"
    ADD CONSTRAINT "PK_Chapters" PRIMARY KEY ("Id");


--
-- Name: Comments PK_Comments; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "PK_Comments" PRIMARY KEY ("Id");


--
-- Name: CourseEnrollments PK_CourseEnrollments; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseEnrollments"
    ADD CONSTRAINT "PK_CourseEnrollments" PRIMARY KEY ("CourseId", "StudentId");


--
-- Name: CourseRatings PK_CourseRatings; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseRatings"
    ADD CONSTRAINT "PK_CourseRatings" PRIMARY KEY ("Id");


--
-- Name: CourseTags PK_CourseTags; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseTags"
    ADD CONSTRAINT "PK_CourseTags" PRIMARY KEY ("Id");


--
-- Name: Courses PK_Courses; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Courses"
    ADD CONSTRAINT "PK_Courses" PRIMARY KEY ("Id");


--
-- Name: Currencies PK_Currencies; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Currencies"
    ADD CONSTRAINT "PK_Currencies" PRIMARY KEY ("Id");


--
-- Name: DiscussionTypes PK_DiscussionTypes; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."DiscussionTypes"
    ADD CONSTRAINT "PK_DiscussionTypes" PRIMARY KEY ("Id");


--
-- Name: Discussions PK_Discussions; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Discussions"
    ADD CONSTRAINT "PK_Discussions" PRIMARY KEY ("Id");


--
-- Name: LessonRatings PK_LessonRatings; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."LessonRatings"
    ADD CONSTRAINT "PK_LessonRatings" PRIMARY KEY ("Id");


--
-- Name: LessonTrackings PK_LessonTrackings; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."LessonTrackings"
    ADD CONSTRAINT "PK_LessonTrackings" PRIMARY KEY ("Id");


--
-- Name: Lessons PK_Lessons; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Lessons"
    ADD CONSTRAINT "PK_Lessons" PRIMARY KEY ("Id");


--
-- Name: Notes PK_Notes; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Notes"
    ADD CONSTRAINT "PK_Notes" PRIMARY KEY ("Id");


--
-- Name: NotificationBellClickLogs PK_NotificationBellClickLogs; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."NotificationBellClickLogs"
    ADD CONSTRAINT "PK_NotificationBellClickLogs" PRIMARY KEY ("Id");


--
-- Name: QuizAnswers PK_QuizAnswers; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."QuizAnswers"
    ADD CONSTRAINT "PK_QuizAnswers" PRIMARY KEY ("Id");


--
-- Name: QuizLessons PK_QuizLessons; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."QuizLessons"
    ADD CONSTRAINT "PK_QuizLessons" PRIMARY KEY ("Id");


--
-- Name: Reactions PK_Reactions; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Reactions"
    ADD CONSTRAINT "PK_Reactions" PRIMARY KEY ("Id");


--
-- Name: SupportRequests PK_SupportRequests; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."SupportRequests"
    ADD CONSTRAINT "PK_SupportRequests" PRIMARY KEY ("Id");


--
-- Name: Tags PK_Tags; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Tags"
    ADD CONSTRAINT "PK_Tags" PRIMARY KEY ("Id");


--
-- Name: UserNotifications PK_UserNotifications; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserNotifications"
    ADD CONSTRAINT "PK_UserNotifications" PRIMARY KEY ("Id");


--
-- Name: VideoLessons PK_VideoLessons; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."VideoLessons"
    ADD CONSTRAINT "PK_VideoLessons" PRIMARY KEY ("Id");


--
-- Name: IX_Bookmarks_CourseId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Bookmarks_CourseId" ON public."Bookmarks" USING btree ("CourseId");


--
-- Name: IX_Bookmarks_LessonId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Bookmarks_LessonId" ON public."Bookmarks" USING btree ("LessonId");


--
-- Name: IX_Bookmarks_UserId_CourseId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Bookmarks_UserId_CourseId" ON public."Bookmarks" USING btree ("UserId", "CourseId");


--
-- Name: IX_Bookmarks_UserId_LessonId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "IX_Bookmarks_UserId_LessonId" ON public."Bookmarks" USING btree ("UserId", "LessonId");


--
-- Name: IX_Chapters_CourseId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Chapters_CourseId" ON public."Chapters" USING btree ("CourseId");


--
-- Name: IX_Comments_DiscussionId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Comments_DiscussionId" ON public."Comments" USING btree ("DiscussionId");


--
-- Name: IX_Comments_ParentId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Comments_ParentId" ON public."Comments" USING btree ("ParentId");


--
-- Name: IX_CourseEnrollments_CourseId_StudentId_LeaveDate; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_CourseEnrollments_CourseId_StudentId_LeaveDate" ON public."CourseEnrollments" USING btree ("CourseId", "StudentId", "LeaveDate");


--
-- Name: IX_CourseEnrollments_StudentId_LeaveDate; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_CourseEnrollments_StudentId_LeaveDate" ON public."CourseEnrollments" USING btree ("StudentId", "LeaveDate");


--
-- Name: IX_CourseRatings_CourseId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_CourseRatings_CourseId" ON public."CourseRatings" USING btree ("CourseId");


--
-- Name: IX_CourseTags_CourseId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_CourseTags_CourseId" ON public."CourseTags" USING btree ("CourseId");


--
-- Name: IX_CourseTags_TagId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_CourseTags_TagId" ON public."CourseTags" USING btree ("TagId");


--
-- Name: IX_Courses_CategoryId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Courses_CategoryId" ON public."Courses" USING btree ("CategoryId");


--
-- Name: IX_Courses_CurrencyId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Courses_CurrencyId" ON public."Courses" USING btree ("CurrencyId");


--
-- Name: IX_Courses_TeacherId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Courses_TeacherId" ON public."Courses" USING btree ("TeacherId");


--
-- Name: IX_Discussions_CourseId_CreatedBy; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Discussions_CourseId_CreatedBy" ON public."Discussions" USING btree ("CourseId", "CreatedBy");


--
-- Name: IX_Discussions_LessonId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Discussions_LessonId" ON public."Discussions" USING btree ("LessonId");


--
-- Name: IX_Discussions_TypeId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Discussions_TypeId" ON public."Discussions" USING btree ("TypeId");


--
-- Name: IX_LessonRatings_LessonId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_LessonRatings_LessonId" ON public."LessonRatings" USING btree ("LessonId");


--
-- Name: IX_LessonTrackings_CourseId_StudentId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_LessonTrackings_CourseId_StudentId" ON public."LessonTrackings" USING btree ("CourseId", "StudentId");


--
-- Name: IX_LessonTrackings_LessonId_StudentId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "IX_LessonTrackings_LessonId_StudentId" ON public."LessonTrackings" USING btree ("LessonId", "StudentId");


--
-- Name: IX_Lessons_ChapterId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Lessons_ChapterId" ON public."Lessons" USING btree ("ChapterId");


--
-- Name: IX_Notes_ChapterId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Notes_ChapterId" ON public."Notes" USING btree ("ChapterId");


--
-- Name: IX_Notes_LessonId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Notes_LessonId" ON public."Notes" USING btree ("LessonId");


--
-- Name: IX_NotificationBellClickLogs_UserId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "IX_NotificationBellClickLogs_UserId" ON public."NotificationBellClickLogs" USING btree ("UserId");


--
-- Name: IX_QuizAnswers_QuizLessonId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_QuizAnswers_QuizLessonId" ON public."QuizAnswers" USING btree ("QuizLessonId");


--
-- Name: IX_QuizLessons_LessonId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "IX_QuizLessons_LessonId" ON public."QuizLessons" USING btree ("LessonId");


--
-- Name: IX_Reactions_CommentId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_Reactions_CommentId" ON public."Reactions" USING btree ("CommentId");


--
-- Name: IX_SupportRequests_FromUserId_Status; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_SupportRequests_FromUserId_Status" ON public."SupportRequests" USING btree ("FromUserId", "Status");


--
-- Name: IX_SupportRequests_FromUserId_Type; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_SupportRequests_FromUserId_Type" ON public."SupportRequests" USING btree ("FromUserId", "Type");


--
-- Name: IX_SupportRequests_ToUserId_Status; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_SupportRequests_ToUserId_Status" ON public."SupportRequests" USING btree ("ToUserId", "Status");


--
-- Name: IX_SupportRequests_ToUserId_Type; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_SupportRequests_ToUserId_Type" ON public."SupportRequests" USING btree ("ToUserId", "Type");


--
-- Name: IX_UserNotifications_CourseId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_UserNotifications_CourseId" ON public."UserNotifications" USING btree ("CourseId");


--
-- Name: IX_UserNotifications_ReceiverId_CourseId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_UserNotifications_ReceiverId_CourseId" ON public."UserNotifications" USING btree ("ReceiverId", "CourseId");


--
-- Name: IX_VideoLessons_LessonId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "IX_VideoLessons_LessonId" ON public."VideoLessons" USING btree ("LessonId");


--
-- Name: Bookmarks FK_Bookmarks_Courses_CourseId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Bookmarks"
    ADD CONSTRAINT "FK_Bookmarks_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id") ON DELETE CASCADE;


--
-- Name: Bookmarks FK_Bookmarks_Lessons_LessonId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Bookmarks"
    ADD CONSTRAINT "FK_Bookmarks_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES public."Lessons"("Id") ON DELETE CASCADE;


--
-- Name: Chapters FK_Chapters_Courses_CourseId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Chapters"
    ADD CONSTRAINT "FK_Chapters_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id") ON DELETE CASCADE;


--
-- Name: Comments FK_Comments_Comments_ParentId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "FK_Comments_Comments_ParentId" FOREIGN KEY ("ParentId") REFERENCES public."Comments"("Id") ON DELETE CASCADE;


--
-- Name: Comments FK_Comments_Discussions_DiscussionId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Comments"
    ADD CONSTRAINT "FK_Comments_Discussions_DiscussionId" FOREIGN KEY ("DiscussionId") REFERENCES public."Discussions"("Id") ON DELETE CASCADE;


--
-- Name: CourseEnrollments FK_CourseEnrollments_Courses_CourseId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseEnrollments"
    ADD CONSTRAINT "FK_CourseEnrollments_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id") ON DELETE RESTRICT;


--
-- Name: CourseRatings FK_CourseRatings_Courses_CourseId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseRatings"
    ADD CONSTRAINT "FK_CourseRatings_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id") ON DELETE CASCADE;


--
-- Name: CourseTags FK_CourseTags_Courses_CourseId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseTags"
    ADD CONSTRAINT "FK_CourseTags_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id") ON DELETE CASCADE;


--
-- Name: CourseTags FK_CourseTags_Tags_TagId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseTags"
    ADD CONSTRAINT "FK_CourseTags_Tags_TagId" FOREIGN KEY ("TagId") REFERENCES public."Tags"("Id") ON DELETE CASCADE;


--
-- Name: Courses FK_Courses_Categories_CategoryId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Courses"
    ADD CONSTRAINT "FK_Courses_Categories_CategoryId" FOREIGN KEY ("CategoryId") REFERENCES public."Categories"("Id") ON DELETE RESTRICT;


--
-- Name: Courses FK_Courses_Currencies_CurrencyId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Courses"
    ADD CONSTRAINT "FK_Courses_Currencies_CurrencyId" FOREIGN KEY ("CurrencyId") REFERENCES public."Currencies"("Id") ON DELETE RESTRICT;


--
-- Name: Discussions FK_Discussions_Courses_CourseId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Discussions"
    ADD CONSTRAINT "FK_Discussions_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id") ON DELETE CASCADE;


--
-- Name: Discussions FK_Discussions_DiscussionTypes_TypeId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Discussions"
    ADD CONSTRAINT "FK_Discussions_DiscussionTypes_TypeId" FOREIGN KEY ("TypeId") REFERENCES public."DiscussionTypes"("Id") ON DELETE RESTRICT;


--
-- Name: Discussions FK_Discussions_Lessons_LessonId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Discussions"
    ADD CONSTRAINT "FK_Discussions_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES public."Lessons"("Id") ON DELETE CASCADE;


--
-- Name: LessonRatings FK_LessonRatings_Lessons_LessonId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."LessonRatings"
    ADD CONSTRAINT "FK_LessonRatings_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES public."Lessons"("Id") ON DELETE CASCADE;


--
-- Name: LessonTrackings FK_LessonTrackings_Courses_CourseId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."LessonTrackings"
    ADD CONSTRAINT "FK_LessonTrackings_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id") ON DELETE RESTRICT;


--
-- Name: LessonTrackings FK_LessonTrackings_Lessons_LessonId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."LessonTrackings"
    ADD CONSTRAINT "FK_LessonTrackings_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES public."Lessons"("Id") ON DELETE CASCADE;


--
-- Name: Lessons FK_Lessons_Chapters_ChapterId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Lessons"
    ADD CONSTRAINT "FK_Lessons_Chapters_ChapterId" FOREIGN KEY ("ChapterId") REFERENCES public."Chapters"("Id") ON DELETE RESTRICT;


--
-- Name: Notes FK_Notes_Chapters_ChapterId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Notes"
    ADD CONSTRAINT "FK_Notes_Chapters_ChapterId" FOREIGN KEY ("ChapterId") REFERENCES public."Chapters"("Id") ON DELETE CASCADE;


--
-- Name: Notes FK_Notes_Lessons_LessonId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Notes"
    ADD CONSTRAINT "FK_Notes_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES public."Lessons"("Id") ON DELETE CASCADE;


--
-- Name: QuizAnswers FK_QuizAnswers_QuizLessons_QuizLessonId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."QuizAnswers"
    ADD CONSTRAINT "FK_QuizAnswers_QuizLessons_QuizLessonId" FOREIGN KEY ("QuizLessonId") REFERENCES public."QuizLessons"("Id") ON DELETE CASCADE;


--
-- Name: QuizLessons FK_QuizLessons_Lessons_LessonId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."QuizLessons"
    ADD CONSTRAINT "FK_QuizLessons_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES public."Lessons"("Id") ON DELETE CASCADE;


--
-- Name: Reactions FK_Reactions_Comments_CommentId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Reactions"
    ADD CONSTRAINT "FK_Reactions_Comments_CommentId" FOREIGN KEY ("CommentId") REFERENCES public."Comments"("Id") ON DELETE CASCADE;


--
-- Name: UserNotifications FK_UserNotifications_Courses_CourseId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserNotifications"
    ADD CONSTRAINT "FK_UserNotifications_Courses_CourseId" FOREIGN KEY ("CourseId") REFERENCES public."Courses"("Id") ON DELETE CASCADE;


--
-- Name: VideoLessons FK_VideoLessons_Lessons_LessonId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."VideoLessons"
    ADD CONSTRAINT "FK_VideoLessons_Lessons_LessonId" FOREIGN KEY ("LessonId") REFERENCES public."Lessons"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict 3NQubuZQTFpgeajCkhdYhlOoMP2fzsI1RAiWzbPmaxAEqSSgMzQKlEdDehLwqBQ

--
-- Database "EduSmart.PaymentManagementService" dump
--

--
-- PostgreSQL database dump
--

\restrict WPBEi1DAOVdaVFd1rTcLKzBtntqQl0BRaSfXQ9pLL8ZgXKtkZljBfVHEfwcNP8A

-- Dumped from database version 17.10 (Debian 17.10-1.pgdg13+1)
-- Dumped by pg_dump version 17.10 (Debian 17.10-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: EduSmart.PaymentManagementService; Type: DATABASE; Schema: -; Owner: pbl6duter
--

CREATE DATABASE "EduSmart.PaymentManagementService" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE "EduSmart.PaymentManagementService" OWNER TO pbl6duter;

\unrestrict WPBEi1DAOVdaVFd1rTcLKzBtntqQl0BRaSfXQ9pLL8ZgXKtkZljBfVHEfwcNP8A
\connect "EduSmart.PaymentManagementService"
\restrict WPBEi1DAOVdaVFd1rTcLKzBtntqQl0BRaSfXQ9pLL8ZgXKtkZljBfVHEfwcNP8A

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: AchievementTemplates; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."AchievementTemplates" (
    "Id" integer NOT NULL,
    "Name" character varying(30) NOT NULL,
    "ThumbnailURL" character varying(200) NOT NULL,
    "TemplateURL" character varying(200) NOT NULL
);


ALTER TABLE public."AchievementTemplates" OWNER TO pbl6duter;

--
-- Name: AchievementTemplates_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."AchievementTemplates" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."AchievementTemplates_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: BankAccounts; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."BankAccounts" (
    "Id" uuid NOT NULL,
    "BankId" integer NOT NULL,
    "UserId" integer NOT NULL,
    "IsAdminAccount" boolean NOT NULL,
    "AccountNumber" character varying(20),
    "AccountName" character varying(255),
    "IsPrimary" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."BankAccounts" OWNER TO pbl6duter;

--
-- Name: Banks; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Banks" (
    "Id" integer NOT NULL,
    "Name" character varying(255) NOT NULL,
    "ShortName" character varying(100) NOT NULL,
    "Bin" character varying(10) NOT NULL,
    "LogoURL" character varying(255),
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."Banks" OWNER TO pbl6duter;

--
-- Name: COLUMN "Banks"."Bin"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."Banks"."Bin" IS 'Bank Identification Number';


--
-- Name: CourseAchievementTemplates; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."CourseAchievementTemplates" (
    "Id" uuid NOT NULL,
    "CourseId" uuid NOT NULL,
    "AchievementTemplateId" integer NOT NULL,
    "CourseNameTextStyle" jsonb,
    "StudentNameTextStyle" jsonb,
    "DateTextStyle" jsonb,
    "TeacherNameTextStyle" jsonb,
    "IsDefault" boolean NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "UpdatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."CourseAchievementTemplates" OWNER TO pbl6duter;

--
-- Name: ExtendStorages; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."ExtendStorages" (
    "Id" uuid NOT NULL,
    "StorageAmount" bigint NOT NULL,
    "Price" numeric NOT NULL,
    "Currency" text NOT NULL,
    "BoughtAt" timestamp with time zone NOT NULL,
    "StorageInfoId" integer NOT NULL
);


ALTER TABLE public."ExtendStorages" OWNER TO pbl6duter;

--
-- Name: COLUMN "ExtendStorages"."StorageAmount"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."ExtendStorages"."StorageAmount" IS 'The amount of storage that is bought';


--
-- Name: COLUMN "ExtendStorages"."Price"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."ExtendStorages"."Price" IS 'The price of the storage at the time of purchase';


--
-- Name: PaymentTransactions; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."PaymentTransactions" (
    "Id" uuid NOT NULL,
    "Code" character varying(12),
    "UserId" integer NOT NULL,
    "CreatorInfo" text,
    "ReceiverId" integer NOT NULL,
    "Amount" numeric NOT NULL,
    "Currency" text NOT NULL,
    "Error" character varying(1000),
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "CompletedAt" timestamp with time zone,
    "PaymentMethod" text NOT NULL,
    "TransactionType" text NOT NULL,
    "OrderStatus" text NOT NULL,
    "RelatedInformation" text
);


ALTER TABLE public."PaymentTransactions" OWNER TO pbl6duter;

--
-- Name: StorageInfos; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."StorageInfos" (
    "Id" integer NOT NULL,
    "UserId" integer NOT NULL,
    "MaximumStorage" bigint NOT NULL,
    "UsedStorage" bigint NOT NULL
);


ALTER TABLE public."StorageInfos" OWNER TO pbl6duter;

--
-- Name: StorageInfos_Id_seq; Type: SEQUENCE; Schema: public; Owner: pbl6duter
--

ALTER TABLE public."StorageInfos" ALTER COLUMN "Id" ADD GENERATED BY DEFAULT AS IDENTITY (
    SEQUENCE NAME public."StorageInfos_Id_seq"
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1
);


--
-- Name: StudentAchievements; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."StudentAchievements" (
    "Id" uuid NOT NULL,
    "StudentId" integer NOT NULL,
    "CourseId" uuid NOT NULL,
    "AchievementURL" character varying(500) NOT NULL,
    "CreatedAt" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL
);


ALTER TABLE public."StudentAchievements" OWNER TO pbl6duter;

--
-- Name: TeacherEarnings; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."TeacherEarnings" (
    "UserId" integer NOT NULL,
    "CurrentBalance" numeric NOT NULL,
    "TotalWithdrawn" numeric NOT NULL
);


ALTER TABLE public."TeacherEarnings" OWNER TO pbl6duter;

--
-- Name: UploadHistories; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."UploadHistories" (
    "Id" uuid NOT NULL,
    "StorageAmount" bigint NOT NULL,
    "ResourceType" text NOT NULL,
    "ResourceId" uuid NOT NULL,
    "UploadDate" timestamp with time zone DEFAULT CURRENT_TIMESTAMP NOT NULL,
    "FileName" character varying(500) NOT NULL,
    "FilePath" character varying(1000) NOT NULL,
    "FileType" text NOT NULL,
    "MediaStorageProvider" text NOT NULL,
    "StorageInfoId" integer NOT NULL
);


ALTER TABLE public."UploadHistories" OWNER TO pbl6duter;

--
-- Name: COLUMN "UploadHistories"."ResourceType"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."UploadHistories"."ResourceType" IS 'The resource type that the file is uploaded for.';


--
-- Name: COLUMN "UploadHistories"."ResourceId"; Type: COMMENT; Schema: public; Owner: pbl6duter
--

COMMENT ON COLUMN public."UploadHistories"."ResourceId" IS 'CourseId, VideoLessonId, AttachmentId, CommentId...';


--
-- Name: WithdrawalRequests; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."WithdrawalRequests" (
    "Id" uuid NOT NULL,
    "UserId" integer NOT NULL,
    "Amount" numeric NOT NULL,
    "Currency" character varying(3) NOT NULL,
    "BankAccountId" uuid NOT NULL,
    "Note" character varying(1000),
    "Status" integer NOT NULL,
    "RequestedAt" timestamp with time zone NOT NULL,
    "ApprovedAt" timestamp with time zone,
    "ApprovedBy" integer NOT NULL,
    "CreatorInfo" jsonb
);


ALTER TABLE public."WithdrawalRequests" OWNER TO pbl6duter;

--
-- Data for Name: AchievementTemplates; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."AchievementTemplates" ("Id", "Name", "ThumbnailURL", "TemplateURL") FROM stdin;
6	Template_06	https://res.cloudinary.com/darzeepog/image/upload/v1783443619/6_kfhzcq.png	https://res.cloudinary.com/darzeepog/image/upload/v1783443619/6_kfhzcq.png
5	Template_05	https://res.cloudinary.com/darzeepog/image/upload/v1783443618/5_vmdpwp.png	https://res.cloudinary.com/darzeepog/image/upload/v1783443618/5_vmdpwp.png
4	Template_04	https://res.cloudinary.com/darzeepog/image/upload/v1783443617/4_doqs2y.png	https://res.cloudinary.com/darzeepog/image/upload/v1783443617/4_doqs2y.png
3	Template_03	https://res.cloudinary.com/darzeepog/image/upload/v1783443618/3_gw6zje.png	https://res.cloudinary.com/darzeepog/image/upload/v1783443618/3_gw6zje.png
2	Template_02	https://res.cloudinary.com/darzeepog/image/upload/v1783443618/2_fvjcpy.png	https://res.cloudinary.com/darzeepog/image/upload/v1783443618/2_fvjcpy.png
1	Template_01	https://res.cloudinary.com/darzeepog/image/upload/v1783443617/1_qqqxnv.png	https://res.cloudinary.com/darzeepog/image/upload/v1783443617/1_qqqxnv.png
\.


--
-- Data for Name: BankAccounts; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."BankAccounts" ("Id", "BankId", "UserId", "IsAdminAccount", "AccountNumber", "AccountName", "IsPrimary", "CreatedAt", "UpdatedAt") FROM stdin;
9118e9db-c0c6-43a4-81c2-18a83430bf54	26	1	t	SEPSMARTEDU2003	Phan Văn Tài	t	2026-07-12 09:50:17.939571+00	2026-07-12 09:50:17.939571+00
6ea4d738-2bdd-4182-aed6-86cdd2df5da7	17	6	f	0905123440	PHAN VĂN TÀI	t	2026-07-12 09:54:49.631555+00	2026-07-12 09:54:49.631555+00
\.


--
-- Data for Name: Banks; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Banks" ("Id", "Name", "ShortName", "Bin", "LogoURL", "CreatedAt", "UpdatedAt") FROM stdin;
1	Ngân hàng TMCP An Bình	ABBANK	970425	https://api.vietqr.io/img/ABB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
2	Ngân hàng TMCP Á Châu	ACB	970416	https://api.vietqr.io/img/ACB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
3	Ngân hàng TMCP Bắc Á	BacABank	970409	https://api.vietqr.io/img/BAB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
4	Ngân hàng TMCP Đầu tư và Phát triển Việt Nam	BIDV	970418	https://api.vietqr.io/img/BIDV.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
5	Ngân hàng TMCP Bảo Việt	BaoVietBank	970438	https://api.vietqr.io/img/BVB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
6	Ngân hàng Thương mại TNHH MTV Xây dựng Việt Nam	CBBank	970444	https://api.vietqr.io/img/CBB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
7	Ngân hàng TNHH MTV CIMB Việt Nam	CIMB	422589	https://api.vietqr.io/img/CIMB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
8	DBS Bank Ltd - Chi nhánh Thành phố Hồ Chí Minh	DBSBank	796500	https://api.vietqr.io/img/DBS.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
9	Ngân hàng TMCP Đông Á	DongABank	970406	https://api.vietqr.io/img/DOB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
10	Ngân hàng TMCP Xuất Nhập khẩu Việt Nam	Eximbank	970431	https://api.vietqr.io/img/EIB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
11	Ngân hàng Thương mại TNHH MTV Dầu Khí Toàn Cầu	GPBank	970408	https://api.vietqr.io/img/GPB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
12	Ngân hàng TMCP Phát triển Thành phố Hồ Chí Minh	HDBank	970437	https://api.vietqr.io/img/HDB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
13	Ngân hàng TNHH MTV Hong Leong Việt Nam	HongLeong	970442	https://api.vietqr.io/img/HLBVN.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
14	Ngân hàng TNHH MTV HSBC (Việt Nam)	HSBC	458761	https://api.vietqr.io/img/HSBC.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
15	Ngân hàng Công nghiệp Hàn Quốc - Chi nhánh Hà Nội	IBKHN	970455	https://api.vietqr.io/img/IBK.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
16	Ngân hàng Công nghiệp Hàn Quốc - Chi nhánh TP. Hồ Chí Minh	IBKHCM	970456	https://api.vietqr.io/img/IBK.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
17	Ngân hàng TMCP Công thương Việt Nam	VietinBank	970415	https://api.vietqr.io/img/ICB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
18	Ngân hàng TNHH Indovina	IndovinaBank	970434	https://api.vietqr.io/img/IVB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
19	Ngân hàng TMCP Kiên Long	KienLongBank	970452	https://api.vietqr.io/img/KLB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
20	Ngân hàng TMCP Lộc Phát Việt Nam	LPBank	970449	https://api.vietqr.io/img/LPB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
21	Ngân hàng TMCP Quân đội	MBBank	970422	https://api.vietqr.io/img/MB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
22	Ngân hàng TMCP Hàng Hải	MSB	970426	https://api.vietqr.io/img/MSB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
23	Ngân hàng TMCP Nam Á	NamABank	970428	https://api.vietqr.io/img/NAB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
24	Ngân hàng TMCP Quốc Dân	NCB	970419	https://api.vietqr.io/img/NCB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
25	Ngân hàng Nonghyup - Chi nhánh Hà Nội	Nonghyup	801011	https://api.vietqr.io/img/NHB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
26	Ngân hàng TMCP Phương Đông	OCB	970448	https://api.vietqr.io/img/OCB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
27	Ngân hàng Thương mại TNHH MTV Đại Dương	Oceanbank	970414	https://api.vietqr.io/img/OCEANBANK.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
28	Ngân hàng TNHH MTV Public Việt Nam	PublicBank	970439	https://api.vietqr.io/img/PBVN.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
29	Ngân hàng TMCP Xăng dầu Petrolimex	PGBank	970430	https://api.vietqr.io/img/PGB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
30	Ngân hàng TMCP Đại Chúng Việt Nam	PVcomBank	970412	https://api.vietqr.io/img/PVCB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
31	Ngân hàng TMCP Sài Gòn	SCB	970429	https://api.vietqr.io/img/SCB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
32	Ngân hàng TNHH MTV Standard Chartered Bank Việt Nam	StandardChartered	970410	https://api.vietqr.io/img/SCVN.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
33	Ngân hàng TMCP Đông Nam Á	SeABank	970440	https://api.vietqr.io/img/SEAB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
34	Ngân hàng TMCP Sài Gòn Công Thương	SaigonBank	970400	https://api.vietqr.io/img/SGICB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
35	Ngân hàng TMCP Sài Gòn - Hà Nội	SHB	970443	https://api.vietqr.io/img/SHB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
36	Ngân hàng TMCP Sài Gòn Thương Tín	Sacombank	970403	https://api.vietqr.io/img/STB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
37	Ngân hàng TNHH MTV Shinhan Việt Nam	ShinhanBank	970424	https://api.vietqr.io/img/SHBVN.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
38	Ngân hàng TMCP Kỹ thương Việt Nam	Techcombank	970407	https://api.vietqr.io/img/TCB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
39	Ngân hàng TMCP Tiên Phong	TPBank	970423	https://api.vietqr.io/img/TPB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
40	Ngân hàng United Overseas - Chi nhánh TP. Hồ Chí Minh	UnitedOverseas	970458	https://api.vietqr.io/img/UOB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
41	Ngân hàng TMCP Việt Á	VietABank	970427	https://api.vietqr.io/img/VAB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
42	Ngân hàng Nông nghiệp và Phát triển Nông thôn Việt Nam	Agribank	970405	https://api.vietqr.io/img/VBA.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
43	Ngân hàng TMCP Ngoại Thương Việt Nam	Vietcombank	970436	https://api.vietqr.io/img/VCB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
44	Ngân hàng TMCP Bản Việt	VietCapitalBank	970454	https://api.vietqr.io/img/VCCB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
45	Ngân hàng TMCP Quốc tế Việt Nam	VIB	970441	https://api.vietqr.io/img/VIB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
46	Ngân hàng TMCP Việt Nam Thương Tín	VietBank	970433	https://api.vietqr.io/img/VIETBANK.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
47	Ngân hàng TMCP Việt Nam Thịnh Vượng	VPBank	970432	https://api.vietqr.io/img/VPB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
48	Ngân hàng Liên doanh Việt - Nga	VRB	970421	https://api.vietqr.io/img/VRB.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
49	Ngân hàng TNHH MTV Woori Việt Nam	Woori	970457	https://api.vietqr.io/img/WVN.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
50	Ngân hàng Kookmin - Chi nhánh Hà Nội	KookminHN	970462	https://api.vietqr.io/img/KBHN.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
51	Ngân hàng Kookmin - Chi nhánh Thành phố Hồ Chí Minh	KookminHCM	970463	https://api.vietqr.io/img/KBHCM.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
52	Ngân hàng Hợp tác xã Việt Nam	COOPBANK	970446	https://api.vietqr.io/img/COOPBANK.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
53	TMCP Việt Nam Thịnh Vượng - Ngân hàng số CAKE by VPBank	CAKE	546034	https://api.vietqr.io/img/CAKE.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
54	TMCP Việt Nam Thịnh Vượng - Ngân hàng số Ubank by VPBank	Ubank	546035	https://api.vietqr.io/img/UBANK.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
55	Ngân hàng Đại chúng TNHH Kasikornbank	KBank	668888	https://api.vietqr.io/img/KBANK.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
56	VNPT Money	VNPTMoney	971011	https://api.vietqr.io/img/VNPTMONEY.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
57	Tổng Công ty Dịch vụ số Viettel - Chi nhánh tập đoàn công nghiệp viễn thông Quân Đội	ViettelMoney	971005	https://api.vietqr.io/img/VIETTELMONEY.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
58	Ngân hàng số Timo by Ban Viet Bank (Timo by Ban Viet Bank)	Timo	963388	https://vietqr.net/portal-service/resources/icons/TIMO.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
59	Ngân hàng Citibank, N.A. - Chi nhánh Hà Nội	Citibank	533948	https://api.vietqr.io/img/CITIBANK.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
60	Ngân hàng KEB Hana – Chi nhánh Thành phố Hồ Chí Minh	KEBHanaHCM	970466	https://api.vietqr.io/img/KEBHANAHCM.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
61	Ngân hàng KEB Hana – Chi nhánh Hà Nội	KEBHANAHN	970467	https://api.vietqr.io/img/KEBHANAHN.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
62	Công ty Tài chính TNHH MTV Mirae Asset (Việt Nam) 	MAFC	977777	https://api.vietqr.io/img/MAFC.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
63	Ngân hàng Chính sách Xã hội	VBSP	999888	https://api.vietqr.io/img/VBSP.png	2026-07-07 07:47:08.188725+00	2026-07-07 07:47:08.188725+00
\.


--
-- Data for Name: CourseAchievementTemplates; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."CourseAchievementTemplates" ("Id", "CourseId", "AchievementTemplateId", "CourseNameTextStyle", "StudentNameTextStyle", "DateTextStyle", "TeacherNameTextStyle", "IsDefault", "CreatedAt", "UpdatedAt") FROM stdin;
5dcf6ecc-17d4-4dae-8dd8-34bfcce7ddfe	518d86a9-1939-4aa1-ae8c-d5e695852170	1	{"color": "#333", "fontSize": "30px", "fontFamily": "Libre Baskerville"}	{"color": "#333", "fontSize": "36px", "fontFamily": "Cinzel"}	{"color": "#333", "fontSize": "30px", "fontFamily": "Lora"}	{"color": "#333", "fontSize": "24px", "fontFamily": "Alex Brush"}	t	2026-07-07 16:38:28.874912+00	2026-07-07 16:38:28.874912+00
\.


--
-- Data for Name: ExtendStorages; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."ExtendStorages" ("Id", "StorageAmount", "Price", "Currency", "BoughtAt", "StorageInfoId") FROM stdin;
\.


--
-- Data for Name: PaymentTransactions; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."PaymentTransactions" ("Id", "Code", "UserId", "CreatorInfo", "ReceiverId", "Amount", "Currency", "Error", "CreatedAt", "CompletedAt", "PaymentMethod", "TransactionType", "OrderStatus", "RelatedInformation") FROM stdin;
d4d080fe-ba29-43f4-b6ed-d4e6c0b16e8c	SEP128527668	2	{"fullName":"T\\u00E0i Phan V\\u0103n","username":"taiphanvan2403","email":"taiphanvan2403@gmail.com","avatarURL":"https://lh3.googleusercontent.com/a/ACg8ocJsGxzIHOhPicxANjrM8yEoH8-VY6KMVRH83FGK1fPOaIVSpvfo=s96-c"}	3	5000	VND	\N	2026-07-07 09:51:41.546655+00	\N	Sepay	BuyCourse	New	{"courseId":"1ee082d2-24b4-425d-9af7-56196fc58460","teacherId":3}
d801d99a-02a1-4b35-919b-da687a3e5f74	SEP462138492	5	{"fullName":"Mike Chi Hao ","username":"mikechihao","email":"mikechihao@gmail.com","avatarURL":"https://lh3.googleusercontent.com/a/ACg8ocLImiFUVeC32fWZi3n2SM2qYN92VGg7rpwHb5_x1--3n1EE=s96-c"}	3	5000	VND	\N	2026-07-07 10:01:12.790039+00	\N	Sepay	BuyCourse	New	{"courseId":"1ee082d2-24b4-425d-9af7-56196fc58460","teacherId":3}
d80f674a-6ad0-4c65-bacf-d3f36765c044	SEP308264299	6	{"fullName":"S\\u01A1n \\u0110\\u1EB7ng","username":"pvt99x","email":"pvt99x@gmail.com","avatarURL":"https://api.dicebear.com/9.x/miniavs/svg?seed=pvt99x"}	3	5000	VND	\N	2026-07-07 17:17:55.44462+00	\N	Sepay	BuyCourse	New	{"courseId":"1ee082d2-24b4-425d-9af7-56196fc58460","teacherId":3}
3fe971a1-52a5-41a2-990f-176622e1b105	SEP774766344	2	{"fullName":"T\\u00E0i Phan V\\u0103n","username":"taiphanvan2403","email":"taiphanvan2403@gmail.com","avatarURL":"https://lh3.googleusercontent.com/a/ACg8ocJsGxzIHOhPicxANjrM8yEoH8-VY6KMVRH83FGK1fPOaIVSpvfo=s96-c"}	6	2000	VND	\N	2026-07-10 04:11:09.540329+00	\N	Sepay	BuyCourse	New	{"courseId":"518d86a9-1939-4aa1-ae8c-d5e695852170","teacherId":6}
7de00de4-3f27-4b11-a354-96497d4d95d9	SEP910171683	4	{"fullName":"L\\u1EADp tr\\u00ECnh Web T\\u1EF1 h\\u1ECDc","username":"anhemdt3","email":"anhemdt3@gmail.com","avatarURL":"https://lh3.googleusercontent.com/a/ACg8ocJTfgaHAgnjL-HvlyrWKxgnX1WoBnoK-DQldLN59it2A1O18_Q=s96-c"}	6	2000	VND	\N	2026-07-12 10:20:51.919772+00	\N	Sepay	BuyCourse	New	{"courseId":"518d86a9-1939-4aa1-ae8c-d5e695852170","teacherId":6}
fadf0ca9-cf76-404e-bad1-9345e1b53177	SEP167261889	4	{"fullName":"L\\u1EADp tr\\u00ECnh Web T\\u1EF1 h\\u1ECDc","username":"anhemdt3","email":"anhemdt3@gmail.com","avatarURL":"https://lh3.googleusercontent.com/a/ACg8ocJTfgaHAgnjL-HvlyrWKxgnX1WoBnoK-DQldLN59it2A1O18_Q=s96-c"}	3	5000	VND	\N	2026-07-12 10:26:16.434919+00	\N	Sepay	BuyCourse	New	{"courseId":"1ee082d2-24b4-425d-9af7-56196fc58460","teacherId":3}
\.


--
-- Data for Name: StorageInfos; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."StorageInfos" ("Id", "UserId", "MaximumStorage", "UsedStorage") FROM stdin;
1	2	524288000	0
2	3	524288000	0
3	4	524288000	0
4	5	524288000	0
\.


--
-- Data for Name: StudentAchievements; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."StudentAchievements" ("Id", "StudentId", "CourseId", "AchievementURL", "CreatedAt") FROM stdin;
\.


--
-- Data for Name: TeacherEarnings; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."TeacherEarnings" ("UserId", "CurrentBalance", "TotalWithdrawn") FROM stdin;
\.


--
-- Data for Name: UploadHistories; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."UploadHistories" ("Id", "StorageAmount", "ResourceType", "ResourceId", "UploadDate", "FileName", "FilePath", "FileType", "MediaStorageProvider", "StorageInfoId") FROM stdin;
\.


--
-- Data for Name: WithdrawalRequests; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."WithdrawalRequests" ("Id", "UserId", "Amount", "Currency", "BankAccountId", "Note", "Status", "RequestedAt", "ApprovedAt", "ApprovedBy", "CreatorInfo") FROM stdin;
\.


--
-- Name: AchievementTemplates_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."AchievementTemplates_Id_seq"', 6, true);


--
-- Name: StorageInfos_Id_seq; Type: SEQUENCE SET; Schema: public; Owner: pbl6duter
--

SELECT pg_catalog.setval('public."StorageInfos_Id_seq"', 4, true);


--
-- Name: AchievementTemplates PK_AchievementTemplates; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."AchievementTemplates"
    ADD CONSTRAINT "PK_AchievementTemplates" PRIMARY KEY ("Id");


--
-- Name: BankAccounts PK_BankAccounts; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."BankAccounts"
    ADD CONSTRAINT "PK_BankAccounts" PRIMARY KEY ("Id");


--
-- Name: Banks PK_Banks; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Banks"
    ADD CONSTRAINT "PK_Banks" PRIMARY KEY ("Id");


--
-- Name: CourseAchievementTemplates PK_CourseAchievementTemplates; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseAchievementTemplates"
    ADD CONSTRAINT "PK_CourseAchievementTemplates" PRIMARY KEY ("Id");


--
-- Name: ExtendStorages PK_ExtendStorages; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."ExtendStorages"
    ADD CONSTRAINT "PK_ExtendStorages" PRIMARY KEY ("Id");


--
-- Name: PaymentTransactions PK_PaymentTransactions; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."PaymentTransactions"
    ADD CONSTRAINT "PK_PaymentTransactions" PRIMARY KEY ("Id");


--
-- Name: StorageInfos PK_StorageInfos; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."StorageInfos"
    ADD CONSTRAINT "PK_StorageInfos" PRIMARY KEY ("Id");


--
-- Name: StudentAchievements PK_StudentAchievements; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."StudentAchievements"
    ADD CONSTRAINT "PK_StudentAchievements" PRIMARY KEY ("Id");


--
-- Name: TeacherEarnings PK_TeacherEarnings; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."TeacherEarnings"
    ADD CONSTRAINT "PK_TeacherEarnings" PRIMARY KEY ("UserId");


--
-- Name: UploadHistories PK_UploadHistories; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UploadHistories"
    ADD CONSTRAINT "PK_UploadHistories" PRIMARY KEY ("Id");


--
-- Name: WithdrawalRequests PK_WithdrawalRequests; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."WithdrawalRequests"
    ADD CONSTRAINT "PK_WithdrawalRequests" PRIMARY KEY ("Id");


--
-- Name: IX_BankAccounts_BankId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_BankAccounts_BankId" ON public."BankAccounts" USING btree ("BankId");


--
-- Name: IX_BankAccounts_IsPrimary_IsAdminAccount; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_BankAccounts_IsPrimary_IsAdminAccount" ON public."BankAccounts" USING btree ("IsPrimary", "IsAdminAccount");


--
-- Name: IX_CourseAchievementTemplates_AchievementTemplateId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_CourseAchievementTemplates_AchievementTemplateId" ON public."CourseAchievementTemplates" USING btree ("AchievementTemplateId");


--
-- Name: IX_CourseAchievementTemplates_CourseId_AchievementTemplateId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "IX_CourseAchievementTemplates_CourseId_AchievementTemplateId" ON public."CourseAchievementTemplates" USING btree ("CourseId", "AchievementTemplateId");


--
-- Name: IX_ExtendStorages_StorageInfoId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_ExtendStorages_StorageInfoId" ON public."ExtendStorages" USING btree ("StorageInfoId");


--
-- Name: IX_PaymentTransactions_ReceiverId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_PaymentTransactions_ReceiverId" ON public."PaymentTransactions" USING btree ("ReceiverId");


--
-- Name: IX_StorageInfos_UserId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "IX_StorageInfos_UserId" ON public."StorageInfos" USING btree ("UserId");


--
-- Name: IX_StudentAchievements_StudentId_CourseId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE UNIQUE INDEX "IX_StudentAchievements_StudentId_CourseId" ON public."StudentAchievements" USING btree ("StudentId", "CourseId");


--
-- Name: IX_UploadHistories_ResourceType_ResourceId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_UploadHistories_ResourceType_ResourceId" ON public."UploadHistories" USING btree ("ResourceType", "ResourceId");


--
-- Name: IX_UploadHistories_StorageInfoId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_UploadHistories_StorageInfoId" ON public."UploadHistories" USING btree ("StorageInfoId");


--
-- Name: IX_WithdrawalRequests_BankAccountId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_WithdrawalRequests_BankAccountId" ON public."WithdrawalRequests" USING btree ("BankAccountId");


--
-- Name: IX_WithdrawalRequests_UserId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_WithdrawalRequests_UserId" ON public."WithdrawalRequests" USING btree ("UserId");


--
-- Name: BankAccounts FK_BankAccounts_Banks_BankId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."BankAccounts"
    ADD CONSTRAINT "FK_BankAccounts_Banks_BankId" FOREIGN KEY ("BankId") REFERENCES public."Banks"("Id") ON DELETE RESTRICT;


--
-- Name: CourseAchievementTemplates FK_CourseAchievementTemplates_AchievementTemplates_Achievement~; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."CourseAchievementTemplates"
    ADD CONSTRAINT "FK_CourseAchievementTemplates_AchievementTemplates_Achievement~" FOREIGN KEY ("AchievementTemplateId") REFERENCES public."AchievementTemplates"("Id") ON DELETE RESTRICT;


--
-- Name: ExtendStorages FK_ExtendStorages_StorageInfos_StorageInfoId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."ExtendStorages"
    ADD CONSTRAINT "FK_ExtendStorages_StorageInfos_StorageInfoId" FOREIGN KEY ("StorageInfoId") REFERENCES public."StorageInfos"("Id") ON DELETE RESTRICT;


--
-- Name: UploadHistories FK_UploadHistories_StorageInfos_StorageInfoId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UploadHistories"
    ADD CONSTRAINT "FK_UploadHistories_StorageInfos_StorageInfoId" FOREIGN KEY ("StorageInfoId") REFERENCES public."StorageInfos"("Id") ON DELETE RESTRICT;


--
-- Name: WithdrawalRequests FK_WithdrawalRequests_BankAccounts_BankAccountId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."WithdrawalRequests"
    ADD CONSTRAINT "FK_WithdrawalRequests_BankAccounts_BankAccountId" FOREIGN KEY ("BankAccountId") REFERENCES public."BankAccounts"("Id") ON DELETE RESTRICT;


--
-- Name: WithdrawalRequests FK_WithdrawalRequests_TeacherEarnings_UserId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."WithdrawalRequests"
    ADD CONSTRAINT "FK_WithdrawalRequests_TeacherEarnings_UserId" FOREIGN KEY ("UserId") REFERENCES public."TeacherEarnings"("UserId") ON DELETE RESTRICT;


--
-- PostgreSQL database dump complete
--

\unrestrict WPBEi1DAOVdaVFd1rTcLKzBtntqQl0BRaSfXQ9pLL8ZgXKtkZljBfVHEfwcNP8A

--
-- Database "EduSmart.UserService" dump
--

--
-- PostgreSQL database dump
--

\restrict aHlt4x0BP7uM7IElb9yP7fS1kq5Ke4fRxOSP4xASXKY4ezjDZQshtb1su3Ysbkv

-- Dumped from database version 17.10 (Debian 17.10-1.pgdg13+1)
-- Dumped by pg_dump version 17.10 (Debian 17.10-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: EduSmart.UserService; Type: DATABASE; Schema: -; Owner: pbl6duter
--

CREATE DATABASE "EduSmart.UserService" WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE "EduSmart.UserService" OWNER TO pbl6duter;

\unrestrict aHlt4x0BP7uM7IElb9yP7fS1kq5Ke4fRxOSP4xASXKY4ezjDZQshtb1su3Ysbkv
\connect "EduSmart.UserService"
\restrict aHlt4x0BP7uM7IElb9yP7fS1kq5Ke4fRxOSP4xASXKY4ezjDZQshtb1su3Ysbkv

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

SET default_tablespace = '';

SET default_table_access_method = heap;

--
-- Name: Roles; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Roles" (
    "Id" integer NOT NULL,
    "Name" text
);


ALTER TABLE public."Roles" OWNER TO pbl6duter;

--
-- Name: UserInfos; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."UserInfos" (
    "UserId" integer NOT NULL,
    "FirstName" character varying(100),
    "LastName" character varying(100),
    "AvatarURL" text,
    "Phone" text,
    "Gender" integer NOT NULL,
    "Bio" character varying(500)
);


ALTER TABLE public."UserInfos" OWNER TO pbl6duter;

--
-- Name: UserRoles; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."UserRoles" (
    "UserId" integer NOT NULL,
    "RoleId" integer NOT NULL
);


ALTER TABLE public."UserRoles" OWNER TO pbl6duter;

--
-- Name: Users; Type: TABLE; Schema: public; Owner: pbl6duter
--

CREATE TABLE public."Users" (
    "Id" integer NOT NULL,
    "UserName" character varying(100) NOT NULL,
    "Email" character varying(100) NOT NULL,
    "IsOnline" boolean NOT NULL,
    "IsActive" boolean NOT NULL,
    "CreatedAt" timestamp with time zone NOT NULL,
    "LastLogin" timestamp with time zone,
    "LastLogout" timestamp with time zone
);


ALTER TABLE public."Users" OWNER TO pbl6duter;

--
-- Data for Name: Roles; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Roles" ("Id", "Name") FROM stdin;
1	Admin
2	Teacher
3	Student
\.


--
-- Data for Name: UserInfos; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."UserInfos" ("UserId", "FirstName", "LastName", "AvatarURL", "Phone", "Gender", "Bio") FROM stdin;
2	Tài	Phan Văn	https://lh3.googleusercontent.com/a/ACg8ocJsGxzIHOhPicxANjrM8yEoH8-VY6KMVRH83FGK1fPOaIVSpvfo=s96-c	\N	0	\N
3	Legal Assistant DUT		https://lh3.googleusercontent.com/a/ACg8ocIFO4vz_dXERtA6OPBm_ZPtIELvcfdmyFJ7NXYVuyu--xjchMM=s96-c	\N	0	\N
4	Lập trình Web	Tự học	https://lh3.googleusercontent.com/a/ACg8ocJTfgaHAgnjL-HvlyrWKxgnX1WoBnoK-DQldLN59it2A1O18_Q=s96-c	\N	0	\N
5	Mike Chi Hao		https://lh3.googleusercontent.com/a/ACg8ocLImiFUVeC32fWZi3n2SM2qYN92VGg7rpwHb5_x1--3n1EE=s96-c	\N	0	\N
6	Sơn	Đặng	https://api.dicebear.com/9.x/miniavs/svg?seed=pvt99x	\N	0	\N
\.


--
-- Data for Name: UserRoles; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."UserRoles" ("UserId", "RoleId") FROM stdin;
2	3
3	2
4	3
5	3
6	2
\.


--
-- Data for Name: Users; Type: TABLE DATA; Schema: public; Owner: pbl6duter
--

COPY public."Users" ("Id", "UserName", "Email", "IsOnline", "IsActive", "CreatedAt", "LastLogin", "LastLogout") FROM stdin;
5	mikechihao	mikechihao@gmail.com	f	t	2026-07-07 10:00:19.181057+00	2026-07-07 10:01:02.016274+00	\N
2	taiphanvan2403	taiphanvan2403@gmail.com	f	t	2026-07-07 08:46:07.874216+00	2026-07-12 06:36:45.723087+00	\N
3	legalassistant.dut	legalassistant.dut@gmail.com	f	t	2026-07-07 08:47:56.769571+00	2026-07-12 06:48:34.618986+00	\N
6	pvt99x	pvt99x@gmail.com	f	t	2026-07-07 10:07:24.969704+00	2026-07-12 09:53:50.58678+00	\N
4	anhemdt3	anhemdt3@gmail.com	f	t	2026-07-07 09:52:02.63487+00	2026-07-12 10:20:40.327532+00	\N
\.


--
-- Name: Roles PK_Roles; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Roles"
    ADD CONSTRAINT "PK_Roles" PRIMARY KEY ("Id");


--
-- Name: UserInfos PK_UserInfos; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserInfos"
    ADD CONSTRAINT "PK_UserInfos" PRIMARY KEY ("UserId");


--
-- Name: UserRoles PK_UserRoles; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "PK_UserRoles" PRIMARY KEY ("UserId", "RoleId");


--
-- Name: Users PK_Users; Type: CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."Users"
    ADD CONSTRAINT "PK_Users" PRIMARY KEY ("Id");


--
-- Name: IX_UserRoles_RoleId; Type: INDEX; Schema: public; Owner: pbl6duter
--

CREATE INDEX "IX_UserRoles_RoleId" ON public."UserRoles" USING btree ("RoleId");


--
-- Name: UserInfos FK_UserInfos_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserInfos"
    ADD CONSTRAINT "FK_UserInfos_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- Name: UserRoles FK_UserRoles_Roles_RoleId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Roles_RoleId" FOREIGN KEY ("RoleId") REFERENCES public."Roles"("Id") ON DELETE CASCADE;


--
-- Name: UserRoles FK_UserRoles_Users_UserId; Type: FK CONSTRAINT; Schema: public; Owner: pbl6duter
--

ALTER TABLE ONLY public."UserRoles"
    ADD CONSTRAINT "FK_UserRoles_Users_UserId" FOREIGN KEY ("UserId") REFERENCES public."Users"("Id") ON DELETE CASCADE;


--
-- PostgreSQL database dump complete
--

\unrestrict aHlt4x0BP7uM7IElb9yP7fS1kq5Ke4fRxOSP4xASXKY4ezjDZQshtb1su3Ysbkv

--
-- Database "pbl6duter" dump
--

--
-- PostgreSQL database dump
--

\restrict 1kYaBPKFivYPRtFOXQRFlVccy9bPbBl5FBca2FE8MB5d2JTnSwsBhdaTsvZoCmQ

-- Dumped from database version 17.10 (Debian 17.10-1.pgdg13+1)
-- Dumped by pg_dump version 17.10 (Debian 17.10-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- Name: pbl6duter; Type: DATABASE; Schema: -; Owner: pbl6duter
--

CREATE DATABASE pbl6duter WITH TEMPLATE = template0 ENCODING = 'UTF8' LOCALE_PROVIDER = libc LOCALE = 'en_US.utf8';


ALTER DATABASE pbl6duter OWNER TO pbl6duter;

\unrestrict 1kYaBPKFivYPRtFOXQRFlVccy9bPbBl5FBca2FE8MB5d2JTnSwsBhdaTsvZoCmQ
\connect pbl6duter
\restrict 1kYaBPKFivYPRtFOXQRFlVccy9bPbBl5FBca2FE8MB5d2JTnSwsBhdaTsvZoCmQ

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- PostgreSQL database dump complete
--

\unrestrict 1kYaBPKFivYPRtFOXQRFlVccy9bPbBl5FBca2FE8MB5d2JTnSwsBhdaTsvZoCmQ

--
-- Database "postgres" dump
--

\connect postgres

--
-- PostgreSQL database dump
--

\restrict 5QojP0nXvULJ9gUFMvama8J01UgGOfLLnrjiyx9FNytdAhR6czxded2Ns7tj4Oz

-- Dumped from database version 17.10 (Debian 17.10-1.pgdg13+1)
-- Dumped by pg_dump version 17.10 (Debian 17.10-1.pgdg13+1)

SET statement_timeout = 0;
SET lock_timeout = 0;
SET idle_in_transaction_session_timeout = 0;
SET transaction_timeout = 0;
SET client_encoding = 'UTF8';
SET standard_conforming_strings = on;
SELECT pg_catalog.set_config('search_path', '', false);
SET check_function_bodies = false;
SET xmloption = content;
SET client_min_messages = warning;
SET row_security = off;

--
-- PostgreSQL database dump complete
--

\unrestrict 5QojP0nXvULJ9gUFMvama8J01UgGOfLLnrjiyx9FNytdAhR6czxded2Ns7tj4Oz

--
-- PostgreSQL database cluster dump complete
--

