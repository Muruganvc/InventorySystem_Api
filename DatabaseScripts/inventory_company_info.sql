CREATE TABLE inventory_company_info (
    inventory_company_info_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    inventory_company_info_name VARCHAR(100) NOT NULL,
    description VARCHAR(150) NOT NULL,
    address VARCHAR(250) NOT NULL,
    mobile_no VARCHAR(10) NOT NULL,
    email VARCHAR(50) NOT NULL,
    gst_number VARCHAR(15) NOT NULL,
    bank_name VARCHAR(50) NOT NULL,
    bank_branch_name VARCHAR(50) NOT NULL,
    bank_account_no VARCHAR(20) NOT NULL,
    bank_branch_ifsc VARCHAR(20) NOT NULL,
    api_version VARCHAR(20) NOT NULL,
    ui_version VARCHAR(20) NOT NULL,
    qr_code BYTEA NOT NULL
);
