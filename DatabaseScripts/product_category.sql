CREATE TABLE product_category (
    product_category_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    product_category_name VARCHAR(100) NOT NULL,
    category_id INT NOT NULL,
    description TEXT,
    is_active BOOLEAN DEFAULT TRUE,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INT NOT NULL,
    modified_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    modified_by INT DEFAULT NULL,

    -- Foreign Keys
    CONSTRAINT fk_productcategory_category FOREIGN KEY (category_id) REFERENCES category(category_id),
    CONSTRAINT fk_productcategory_createdby FOREIGN KEY (created_by) REFERENCES users(user_id),
    CONSTRAINT fk_productcategory_modifiedby FOREIGN KEY (modified_by) REFERENCES users(user_id)
);
