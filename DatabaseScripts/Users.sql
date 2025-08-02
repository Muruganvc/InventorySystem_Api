CREATE TABLE users (
    user_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    first_name VARCHAR(30) NOT NULL,
    last_name VARCHAR(30),
    user_name VARCHAR(30) NOT NULL UNIQUE,
    password_hash VARCHAR(256) NOT NULL,
    email VARCHAR(150),
    is_active BOOLEAN NOT NULL DEFAULT TRUE,
    password_last_changed TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,

    -- Computed column: expires 30 days after last password change
    password_expires_at TIMESTAMP GENERATED ALWAYS AS (
        password_last_changed + INTERVAL '30 days'
    ) STORED,

    is_password_expired BOOLEAN NOT NULL DEFAULT FALSE,
    last_login TIMESTAMP,
    mobile_no VARCHAR(10) NOT NULL,
    profile_image BYTEA,
    created_by INT NOT NULL,
    created_date TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    modified_by INT,
    modified_date TIMESTAMP
);
