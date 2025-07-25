CREATE TABLE category (
    category_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    category_name VARCHAR(100) NOT NULL,
    company_id INT,
    description TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INT NOT NULL,
    modified_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_by INT DEFAULT NULL,

    -- Foreign Keys
    CONSTRAINT fk_category_company FOREIGN KEY (company_id) REFERENCES company(company_id),
    CONSTRAINT fk_category_createdby FOREIGN KEY (created_by) REFERENCES users(user_id),
    CONSTRAINT fk_category_modifiedby FOREIGN KEY (modified_by) REFERENCES users(user_id)
);
