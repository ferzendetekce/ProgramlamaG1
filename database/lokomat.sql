--
-- PostgreSQL database dump
--

\restrict ZbCanGxX9BYSiEQsXBMsEX08RWRdq35hkqm2lfaFM1ZfRkiLi7fxhH2rHSheyB4

-- Dumped from database version 18.0
-- Dumped by pg_dump version 18.0

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
-- Name: cinsiyetler; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.cinsiyetler (
    cinsiyet_id integer NOT NULL,
    cinsiyet_adi character varying(50) NOT NULL
);


ALTER TABLE public.cinsiyetler OWNER TO postgres;

--
-- Name: cinsiyetler_cinsiyet_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.cinsiyetler_cinsiyet_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.cinsiyetler_cinsiyet_id_seq OWNER TO postgres;

--
-- Name: cinsiyetler_cinsiyet_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.cinsiyetler_cinsiyet_id_seq OWNED BY public.cinsiyetler.cinsiyet_id;


--
-- Name: hastalar; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.hastalar (
    hasta_id integer NOT NULL,
    tc_kimlik_no character varying(11) NOT NULL,
    ad character varying(100) NOT NULL,
    soyad character varying(100) NOT NULL,
    dogum_tarihi date,
    cinsiyet_id integer,
    boy_cm integer,
    kilo_kg numeric(5,2),
    bacak_boyu_cm integer,
    kalca_diz_boyu_cm integer,
    diz_bilek_boyu_cm integer,
    ayak_numarasi integer,
    teshis character varying(255),
    teshis_aciklamasi text,
    rahatsizlandigi_tarih date,
    telefon character varying(20),
    e_posta character varying(100),
    adres text,
    sehir_plaka_kodu integer,
    olusturulma_tarihi timestamp with time zone DEFAULT now(),
    son_giris_tarihi timestamp with time zone
);


ALTER TABLE public.hastalar OWNER TO postgres;

--
-- Name: hastalar_hasta_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.hastalar_hasta_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.hastalar_hasta_id_seq OWNER TO postgres;

--
-- Name: hastalar_hasta_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.hastalar_hasta_id_seq OWNED BY public.hastalar.hasta_id;


--
-- Name: kullanicihesaplari; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.kullanicihesaplari (
    hesap_id integer NOT NULL,
    personel_id integer,
    rol_id integer NOT NULL,
    kullanici_adi character varying(50) NOT NULL,
    sifre_hash character varying(255) NOT NULL,
    aktif_mi boolean DEFAULT true,
    olusturulma_tarihi timestamp with time zone DEFAULT now(),
    son_giris_tarihi timestamp with time zone
);


ALTER TABLE public.kullanicihesaplari OWNER TO postgres;

--
-- Name: kullanicihesaplari_hesap_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.kullanicihesaplari_hesap_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.kullanicihesaplari_hesap_id_seq OWNER TO postgres;

--
-- Name: kullanicihesaplari_hesap_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.kullanicihesaplari_hesap_id_seq OWNED BY public.kullanicihesaplari.hesap_id;


--
-- Name: loglar; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.loglar (
    log_id bigint NOT NULL,
    log_seviyesi character varying(20) NOT NULL,
    mesaj text NOT NULL,
    olay_zamani timestamp with time zone DEFAULT now(),
    hesap_id integer,
    ip_adresi character varying(45),
    CONSTRAINT loglar_log_seviyesi_check CHECK (((log_seviyesi)::text = ANY ((ARRAY['INFO'::character varying, 'WARNING'::character varying, 'ERROR'::character varying, 'CRITICAL'::character varying])::text[])))
);


ALTER TABLE public.loglar OWNER TO postgres;

--
-- Name: loglar_log_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.loglar_log_id_seq
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.loglar_log_id_seq OWNER TO postgres;

--
-- Name: loglar_log_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.loglar_log_id_seq OWNED BY public.loglar.log_id;


--
-- Name: personeller; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.personeller (
    personel_id integer NOT NULL,
    ad character varying(100) NOT NULL,
    soyad character varying(100) NOT NULL,
    telefon character varying(20),
    e_posta character varying(100),
    adres text,
    sehir_plaka_kodu integer,
    personel_tip_id integer NOT NULL,
    kayit_tarihi timestamp with time zone DEFAULT now()
);


ALTER TABLE public.personeller OWNER TO postgres;

--
-- Name: personeller_personel_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.personeller_personel_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.personeller_personel_id_seq OWNER TO postgres;

--
-- Name: personeller_personel_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.personeller_personel_id_seq OWNED BY public.personeller.personel_id;


--
-- Name: personeltipleri; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.personeltipleri (
    personel_tip_id integer NOT NULL,
    tip_adi character varying(50) NOT NULL
);


ALTER TABLE public.personeltipleri OWNER TO postgres;

--
-- Name: personeltipleri_personel_tip_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.personeltipleri_personel_tip_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.personeltipleri_personel_tip_id_seq OWNER TO postgres;

--
-- Name: personeltipleri_personel_tip_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.personeltipleri_personel_tip_id_seq OWNED BY public.personeltipleri.personel_tip_id;


--
-- Name: roller; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.roller (
    rol_id integer NOT NULL,
    rol_adi character varying(50) NOT NULL
);


ALTER TABLE public.roller OWNER TO postgres;

--
-- Name: roller_rol_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.roller_rol_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.roller_rol_id_seq OWNER TO postgres;

--
-- Name: roller_rol_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.roller_rol_id_seq OWNED BY public.roller.rol_id;


--
-- Name: rolyetkileri; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.rolyetkileri (
    rol_id integer NOT NULL,
    yetki_id integer NOT NULL
);


ALTER TABLE public.rolyetkileri OWNER TO postgres;

--
-- Name: seanslar; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.seanslar (
    seans_id integer NOT NULL,
    hasta_id integer NOT NULL,
    personel_id integer NOT NULL,
    seans_tarihi_baslangic timestamp with time zone DEFAULT now(),
    seans_suresi_dk integer,
    yurume_hizi_km_s numeric(4,2),
    vucut_agirlik_destegi_yuzde integer,
    yurunen_mesafe_m integer,
    zorluk_seviyesi character varying(50),
    ortalama_kalp_atisi_bpm integer,
    notlar text
);


ALTER TABLE public.seanslar OWNER TO postgres;

--
-- Name: seanslar_seans_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.seanslar_seans_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.seanslar_seans_id_seq OWNER TO postgres;

--
-- Name: seanslar_seans_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.seanslar_seans_id_seq OWNED BY public.seanslar.seans_id;


--
-- Name: sehirler; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.sehirler (
    plaka_kodu integer NOT NULL,
    sehir_adi character varying(100) NOT NULL
);


ALTER TABLE public.sehirler OWNER TO postgres;

--
-- Name: yetkiler; Type: TABLE; Schema: public; Owner: postgres
--

CREATE TABLE public.yetkiler (
    yetki_id integer NOT NULL,
    yetki_kodu character varying(100) NOT NULL,
    aciklama text
);


ALTER TABLE public.yetkiler OWNER TO postgres;

--
-- Name: yetkiler_yetki_id_seq; Type: SEQUENCE; Schema: public; Owner: postgres
--

CREATE SEQUENCE public.yetkiler_yetki_id_seq
    AS integer
    START WITH 1
    INCREMENT BY 1
    NO MINVALUE
    NO MAXVALUE
    CACHE 1;


ALTER SEQUENCE public.yetkiler_yetki_id_seq OWNER TO postgres;

--
-- Name: yetkiler_yetki_id_seq; Type: SEQUENCE OWNED BY; Schema: public; Owner: postgres
--

ALTER SEQUENCE public.yetkiler_yetki_id_seq OWNED BY public.yetkiler.yetki_id;


--
-- Name: cinsiyetler cinsiyet_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cinsiyetler ALTER COLUMN cinsiyet_id SET DEFAULT nextval('public.cinsiyetler_cinsiyet_id_seq'::regclass);


--
-- Name: hastalar hasta_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hastalar ALTER COLUMN hasta_id SET DEFAULT nextval('public.hastalar_hasta_id_seq'::regclass);


--
-- Name: kullanicihesaplari hesap_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.kullanicihesaplari ALTER COLUMN hesap_id SET DEFAULT nextval('public.kullanicihesaplari_hesap_id_seq'::regclass);


--
-- Name: loglar log_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.loglar ALTER COLUMN log_id SET DEFAULT nextval('public.loglar_log_id_seq'::regclass);


--
-- Name: personeller personel_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.personeller ALTER COLUMN personel_id SET DEFAULT nextval('public.personeller_personel_id_seq'::regclass);


--
-- Name: personeltipleri personel_tip_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.personeltipleri ALTER COLUMN personel_tip_id SET DEFAULT nextval('public.personeltipleri_personel_tip_id_seq'::regclass);


--
-- Name: roller rol_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roller ALTER COLUMN rol_id SET DEFAULT nextval('public.roller_rol_id_seq'::regclass);


--
-- Name: seanslar seans_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.seanslar ALTER COLUMN seans_id SET DEFAULT nextval('public.seanslar_seans_id_seq'::regclass);


--
-- Name: yetkiler yetki_id; Type: DEFAULT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.yetkiler ALTER COLUMN yetki_id SET DEFAULT nextval('public.yetkiler_yetki_id_seq'::regclass);


--
-- Data for Name: cinsiyetler; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.cinsiyetler (cinsiyet_id, cinsiyet_adi) FROM stdin;
1	Erkek
2	Kadın
3	Belirtilmemiş
\.


--
-- Data for Name: hastalar; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.hastalar (hasta_id, tc_kimlik_no, ad, soyad, dogum_tarihi, cinsiyet_id, boy_cm, kilo_kg, bacak_boyu_cm, kalca_diz_boyu_cm, diz_bilek_boyu_cm, ayak_numarasi, teshis, teshis_aciklamasi, rahatsizlandigi_tarih, telefon, e_posta, adres, sehir_plaka_kodu, olusturulma_tarihi, son_giris_tarihi) FROM stdin;
1	11122233344	Ahmet	Yılmaz	1985-06-15	1	180	85.50	95	50	45	43	İnme Sonrası Hemiparezi	\N	2025-01-20	5551112233	ahmet.yilmaz@example.com	Kızılay Mah. Atatürk Blv. No:1	6	2025-10-13 23:59:33.154776+03	\N
2	55566677788	Zeynep	Kaya	1992-11-30	2	165	62.00	85	45	40	38	Serebral Palsi	\N	1993-02-10	5554445566	zeynep.kaya@example.com	Kadıköy Mah. Bahariye Cad. No:50	34	2025-10-13 23:59:33.154776+03	\N
\.


--
-- Data for Name: kullanicihesaplari; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.kullanicihesaplari (hesap_id, personel_id, rol_id, kullanici_adi, sifre_hash, aktif_mi, olusturulma_tarihi, son_giris_tarihi) FROM stdin;
1	1	2	egunes	$2a$12$D9gO8O.yL3e/G0d.z4c.yuzM1V3cE6s.b8.t5XzQ9Pq2XzQ9Pq2Xz	t	2025-10-13 23:59:33.170878+03	\N
2	3	1	admin	$2a$12$K8lP.t.o9p.q1.x3.f7.z8.a.b.c.d.e.f.g.h.i.j.k.l.m.n	t	2025-10-13 23:59:33.170878+03	\N
\.


--
-- Data for Name: loglar; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.loglar (log_id, log_seviyesi, mesaj, olay_zamani, hesap_id, ip_adresi) FROM stdin;
1	INFO	Kullanıcı [admin] sisteme başarıyla giriş yaptı.	2025-10-13 23:59:33.178187+03	2	192.168.1.100
\.


--
-- Data for Name: personeller; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.personeller (personel_id, ad, soyad, telefon, e_posta, adres, sehir_plaka_kodu, personel_tip_id, kayit_tarihi) FROM stdin;
1	Elif	Güneş	5051234567	elif.gunes@clinicsystem.com	Çankaya Mah. No:10	6	2	2025-10-13 23:59:33.160581+03
2	Mehmet	Öztürk	5069876543	mehmet.ozturk@clinicsystem.com	Beşiktaş Mah. No:25	34	1	2025-10-13 23:59:33.160581+03
3	Ayşe	Demir	5075558899	ayse.demir@clinicsystem.com	Alsancak Mah. No:12	35	3	2025-10-13 23:59:33.160581+03
\.


--
-- Data for Name: personeltipleri; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.personeltipleri (personel_tip_id, tip_adi) FROM stdin;
1	Doktor
2	Fizyoterapist
3	Yönetici
\.


--
-- Data for Name: roller; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.roller (rol_id, rol_adi) FROM stdin;
1	Admin
2	Terapist
\.


--
-- Data for Name: rolyetkileri; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.rolyetkileri (rol_id, yetki_id) FROM stdin;
2	1
2	3
1	1
1	2
1	3
1	4
\.


--
-- Data for Name: seanslar; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.seanslar (seans_id, hasta_id, personel_id, seans_tarihi_baslangic, seans_suresi_dk, yurume_hizi_km_s, vucut_agirlik_destegi_yuzde, yurunen_mesafe_m, zorluk_seviyesi, ortalama_kalp_atisi_bpm, notlar) FROM stdin;
1	2	1	2025-10-13 23:59:33.175318+03	45	1.20	35	550	Orta	\N	Hastanın motivasyonu yüksek. Sağ bacak adımlama simetrisinde %10 artış gözlendi.
\.


--
-- Data for Name: sehirler; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.sehirler (plaka_kodu, sehir_adi) FROM stdin;
1	Adana
2	Adıyaman
3	Afyonkarahisar
4	Ağrı
5	Amasya
6	Ankara
7	Antalya
8	Artvin
9	Aydın
10	Balıkesir
11	Bilecik
12	Bingöl
13	Bitlis
14	Bolu
15	Burdur
16	Bursa
17	Çanakkale
18	Çankırı
19	Çorum
20	Denizli
21	Diyarbakır
22	Edirne
23	Elazığ
24	Erzincan
25	Erzurum
26	Eskişehir
27	Gaziantep
28	Giresun
29	Gümüşhane
30	Hakkari
31	Hatay
32	Isparta
33	Mersin
34	İstanbul
35	İzmir
36	Kars
37	Kastamonu
38	Kayseri
39	Kırklareli
40	Kırşehir
41	Kocaeli
42	Konya
43	Kütahya
44	Malatya
45	Manisa
46	Kahramanmaraş
47	Mardin
48	Muğla
49	Muş
50	Nevşehir
51	Niğde
52	Ordu
53	Rize
54	Sakarya
55	Samsun
56	Siirt
57	Sinop
58	Sivas
59	Tekirdağ
60	Tokat
61	Trabzon
62	Tunceli
63	Şanlıurfa
64	Uşak
65	Van
66	Yozgat
67	Zonguldak
68	Aksaray
69	Bayburt
70	Karaman
71	Kırıkkale
72	Batman
73	Şırnak
74	Bartın
75	Ardahan
76	Iğdır
77	Yalova
78	Karabük
79	Kilis
80	Osmaniye
81	Düzce
\.


--
-- Data for Name: yetkiler; Type: TABLE DATA; Schema: public; Owner: postgres
--

COPY public.yetkiler (yetki_id, yetki_kodu, aciklama) FROM stdin;
1	HASTA_GORUNTULE	Hasta kayıtlarını listeler ve detaylarını görüntüler.
2	HASTA_YONETIMI	Yeni hasta oluşturur, düzenler ve siler.
3	SEANS_YONETIMI	Hastalar için seans başlatır, bitirir ve düzenler.
4	KULLANICI_YONETIMI	Sistem kullanıcılarını ve rollerini yönetir.
\.


--
-- Name: cinsiyetler_cinsiyet_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.cinsiyetler_cinsiyet_id_seq', 3, true);


--
-- Name: hastalar_hasta_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.hastalar_hasta_id_seq', 2, true);


--
-- Name: kullanicihesaplari_hesap_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.kullanicihesaplari_hesap_id_seq', 2, true);


--
-- Name: loglar_log_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.loglar_log_id_seq', 1, true);


--
-- Name: personeller_personel_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.personeller_personel_id_seq', 3, true);


--
-- Name: personeltipleri_personel_tip_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.personeltipleri_personel_tip_id_seq', 3, true);


--
-- Name: roller_rol_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.roller_rol_id_seq', 2, true);


--
-- Name: seanslar_seans_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.seanslar_seans_id_seq', 1, true);


--
-- Name: yetkiler_yetki_id_seq; Type: SEQUENCE SET; Schema: public; Owner: postgres
--

SELECT pg_catalog.setval('public.yetkiler_yetki_id_seq', 4, true);


--
-- Name: cinsiyetler cinsiyetler_cinsiyet_adi_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cinsiyetler
    ADD CONSTRAINT cinsiyetler_cinsiyet_adi_key UNIQUE (cinsiyet_adi);


--
-- Name: cinsiyetler cinsiyetler_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.cinsiyetler
    ADD CONSTRAINT cinsiyetler_pkey PRIMARY KEY (cinsiyet_id);


--
-- Name: hastalar hastalar_e_posta_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hastalar
    ADD CONSTRAINT hastalar_e_posta_key UNIQUE (e_posta);


--
-- Name: hastalar hastalar_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hastalar
    ADD CONSTRAINT hastalar_pkey PRIMARY KEY (hasta_id);


--
-- Name: hastalar hastalar_tc_kimlik_no_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hastalar
    ADD CONSTRAINT hastalar_tc_kimlik_no_key UNIQUE (tc_kimlik_no);


--
-- Name: kullanicihesaplari kullanicihesaplari_kullanici_adi_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.kullanicihesaplari
    ADD CONSTRAINT kullanicihesaplari_kullanici_adi_key UNIQUE (kullanici_adi);


--
-- Name: kullanicihesaplari kullanicihesaplari_personel_id_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.kullanicihesaplari
    ADD CONSTRAINT kullanicihesaplari_personel_id_key UNIQUE (personel_id);


--
-- Name: kullanicihesaplari kullanicihesaplari_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.kullanicihesaplari
    ADD CONSTRAINT kullanicihesaplari_pkey PRIMARY KEY (hesap_id);


--
-- Name: loglar loglar_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.loglar
    ADD CONSTRAINT loglar_pkey PRIMARY KEY (log_id);


--
-- Name: personeller personeller_e_posta_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.personeller
    ADD CONSTRAINT personeller_e_posta_key UNIQUE (e_posta);


--
-- Name: personeller personeller_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.personeller
    ADD CONSTRAINT personeller_pkey PRIMARY KEY (personel_id);


--
-- Name: personeltipleri personeltipleri_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.personeltipleri
    ADD CONSTRAINT personeltipleri_pkey PRIMARY KEY (personel_tip_id);


--
-- Name: personeltipleri personeltipleri_tip_adi_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.personeltipleri
    ADD CONSTRAINT personeltipleri_tip_adi_key UNIQUE (tip_adi);


--
-- Name: roller roller_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roller
    ADD CONSTRAINT roller_pkey PRIMARY KEY (rol_id);


--
-- Name: roller roller_rol_adi_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.roller
    ADD CONSTRAINT roller_rol_adi_key UNIQUE (rol_adi);


--
-- Name: rolyetkileri rolyetkileri_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rolyetkileri
    ADD CONSTRAINT rolyetkileri_pkey PRIMARY KEY (rol_id, yetki_id);


--
-- Name: seanslar seanslar_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.seanslar
    ADD CONSTRAINT seanslar_pkey PRIMARY KEY (seans_id);


--
-- Name: sehirler sehirler_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.sehirler
    ADD CONSTRAINT sehirler_pkey PRIMARY KEY (plaka_kodu);


--
-- Name: sehirler sehirler_sehir_adi_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.sehirler
    ADD CONSTRAINT sehirler_sehir_adi_key UNIQUE (sehir_adi);


--
-- Name: yetkiler yetkiler_pkey; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.yetkiler
    ADD CONSTRAINT yetkiler_pkey PRIMARY KEY (yetki_id);


--
-- Name: yetkiler yetkiler_yetki_kodu_key; Type: CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.yetkiler
    ADD CONSTRAINT yetkiler_yetki_kodu_key UNIQUE (yetki_kodu);


--
-- Name: hastalar fk_hasta_cinsiyet; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hastalar
    ADD CONSTRAINT fk_hasta_cinsiyet FOREIGN KEY (cinsiyet_id) REFERENCES public.cinsiyetler(cinsiyet_id);


--
-- Name: hastalar fk_hasta_sehir; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.hastalar
    ADD CONSTRAINT fk_hasta_sehir FOREIGN KEY (sehir_plaka_kodu) REFERENCES public.sehirler(plaka_kodu);


--
-- Name: personeller fk_personel_sehir; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.personeller
    ADD CONSTRAINT fk_personel_sehir FOREIGN KEY (sehir_plaka_kodu) REFERENCES public.sehirler(plaka_kodu);


--
-- Name: personeller fk_personel_tip; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.personeller
    ADD CONSTRAINT fk_personel_tip FOREIGN KEY (personel_tip_id) REFERENCES public.personeltipleri(personel_tip_id);


--
-- Name: kullanicihesaplari kullanicihesaplari_personel_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.kullanicihesaplari
    ADD CONSTRAINT kullanicihesaplari_personel_id_fkey FOREIGN KEY (personel_id) REFERENCES public.personeller(personel_id) ON DELETE SET NULL;


--
-- Name: kullanicihesaplari kullanicihesaplari_rol_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.kullanicihesaplari
    ADD CONSTRAINT kullanicihesaplari_rol_id_fkey FOREIGN KEY (rol_id) REFERENCES public.roller(rol_id) ON DELETE RESTRICT;


--
-- Name: loglar loglar_hesap_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.loglar
    ADD CONSTRAINT loglar_hesap_id_fkey FOREIGN KEY (hesap_id) REFERENCES public.kullanicihesaplari(hesap_id) ON DELETE SET NULL;


--
-- Name: rolyetkileri rolyetkileri_rol_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rolyetkileri
    ADD CONSTRAINT rolyetkileri_rol_id_fkey FOREIGN KEY (rol_id) REFERENCES public.roller(rol_id) ON DELETE CASCADE;


--
-- Name: rolyetkileri rolyetkileri_yetki_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.rolyetkileri
    ADD CONSTRAINT rolyetkileri_yetki_id_fkey FOREIGN KEY (yetki_id) REFERENCES public.yetkiler(yetki_id) ON DELETE CASCADE;


--
-- Name: seanslar seanslar_hasta_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.seanslar
    ADD CONSTRAINT seanslar_hasta_id_fkey FOREIGN KEY (hasta_id) REFERENCES public.hastalar(hasta_id) ON DELETE CASCADE;


--
-- Name: seanslar seanslar_personel_id_fkey; Type: FK CONSTRAINT; Schema: public; Owner: postgres
--

ALTER TABLE ONLY public.seanslar
    ADD CONSTRAINT seanslar_personel_id_fkey FOREIGN KEY (personel_id) REFERENCES public.personeller(personel_id) ON DELETE RESTRICT;


--
-- PostgreSQL database dump complete
--

\unrestrict ZbCanGxX9BYSiEQsXBMsEX08RWRdq35hkqm2lfaFM1ZfRkiLi7fxhH2rHSheyB4

