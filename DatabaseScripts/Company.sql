CREATE TABLE company (
    company_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    company_name VARCHAR(100) NOT NULL,
    description TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER NOT NULL,
    modified_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_by INTEGER DEFAULT NULL,

    CONSTRAINT fk_company_created_by FOREIGN KEY (created_by) REFERENCES users(user_id),
    CONSTRAINT fk_company_modified_by FOREIGN KEY (modified_by) REFERENCES users(user_id)
);
