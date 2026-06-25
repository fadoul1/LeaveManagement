-- Exécuté automatiquement par PostgreSQL au 1er démarrage du container
-- (uniquement si le volume postgres_data est vide)

-- Schéma pour Keycloak
CREATE SCHEMA IF NOT EXISTS keycloak;

-- Schéma public déjà existant par défaut, mais on s'assure des droits
GRANT ALL ON SCHEMA public TO postgres;
GRANT ALL ON SCHEMA keycloak TO postgres;