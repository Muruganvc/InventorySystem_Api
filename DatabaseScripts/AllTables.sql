-- Drop and create roles table
DROP TABLE IF EXISTS roles CASCADE;
CREATE TABLE roles (
    role_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    is_active boolean DEFAULT true,
    role_name character varying(100) UNIQUE,
    role_code character varying(50) UNIQUE
);

-- Drop and create users table
DROP TABLE IF EXISTS users CASCADE;
CREATE TABLE users (
    user_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    first_name character varying(30),
    last_name character varying(30),
    user_name character varying(30) UNIQUE,
    password_hash character varying(256),
    email character varying(150),
    mobile_no character varying(10),
    password_last_changed timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    password_expires_at timestamp without time zone,
    is_password_expired boolean DEFAULT false,
    is_active boolean DEFAULT true,
    created_by integer,
    created_date timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_by integer,
    modified_date timestamp without time zone,
    last_login timestamp without time zone,
    profile_image bytea
);

-- Drop and create user_roles table (Ensure users and roles tables are created first)
DROP TABLE IF EXISTS user_roles CASCADE;
CREATE TABLE user_roles (
    user_id INT,
    role_id INT,
    PRIMARY KEY (user_id, role_id),
    CONSTRAINT user_roles_user_id_fkey FOREIGN KEY (user_id) REFERENCES users(user_id) ON DELETE CASCADE,
    CONSTRAINT user_roles_role_id_fkey FOREIGN KEY (role_id) REFERENCES roles(role_id) ON DELETE CASCADE
);

-- Drop and create inventory_company_info table
DROP TABLE IF EXISTS inventory_company_info CASCADE;
CREATE TABLE inventory_company_info (
    inventory_company_info_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    qr_code bytea,
    is_active boolean DEFAULT false,
    address character varying(250),
    mobile_no character varying(10),
    email character varying(50),
    gst_number character varying(15),
    bank_name character varying(50),
    bank_branch_name character varying(50),
    bank_account_no character varying(20),
    bank_branch_ifsc character varying(20),
    api_version character varying(20),
    ui_version character varying(20),
    inventory_company_info_name character varying(100),
    description character varying(150)
);

-- Drop and create customers table
DROP TABLE IF EXISTS customers CASCADE;
CREATE TABLE customers (
    customer_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    customer_name character varying(200),
    phone character varying(20),
    address text,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP
);

-- Drop and create menu_items table
DROP TABLE IF EXISTS menu_items CASCADE;
CREATE TABLE menu_items (
    menu_items_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    parent_id integer,
    order_by integer DEFAULT 0,
    label character varying(100),
    icon character varying(50),
    route character varying(200),
    CONSTRAINT fk_menuitems_parent FOREIGN KEY (parent_id) REFERENCES menu_items(menu_items_id) ON DELETE SET NULL
);

-- Drop and create audit_logs table
DROP TABLE IF EXISTS audit_logs CASCADE;
CREATE TABLE audit_logs (
    audit_log_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    changed_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    action character varying(50),
    key_values text,
    old_values text,
    new_values text,
    changed_by character varying(100),
    table_name character varying(100)
);

-- Drop and create company table
DROP TABLE IF EXISTS company CASCADE;
CREATE TABLE company (
    company_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    company_name character varying(100),
    description text,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_by integer,
    CONSTRAINT fk_company_created_by FOREIGN KEY (created_by) REFERENCES users(user_id),
    CONSTRAINT fk_company_modified_by FOREIGN KEY (modified_by) REFERENCES users(user_id)
);

-- Drop and create category table
DROP TABLE IF EXISTS category CASCADE;
CREATE TABLE category (
    category_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    category_name character varying(100),
    description text,
    company_id integer,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_by integer,
    CONSTRAINT fk_category_company FOREIGN KEY (company_id) REFERENCES company(company_id),
    CONSTRAINT fk_category_createdby FOREIGN KEY (created_by) REFERENCES users(user_id),
    CONSTRAINT fk_category_modifiedby FOREIGN KEY (modified_by) REFERENCES users(user_id)
);

-- Drop and create orders table
DROP TABLE IF EXISTS orders CASCADE;
CREATE TABLE orders (
    order_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    customer_id integer,
    order_date timestamp with time zone DEFAULT CURRENT_TIMESTAMP,
    total_amount numeric,
    final_amount numeric,
    balance_amount numeric,
    is_gst boolean DEFAULT false,
    gst_number character varying(15),
    CONSTRAINT fk_orders_customer FOREIGN KEY (customer_id) REFERENCES customers(customer_id) ON DELETE CASCADE
);

-- Drop and create product_category table
DROP TABLE IF EXISTS product_category CASCADE;
CREATE TABLE product_category (
    product_category_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    product_category_name character varying(100),
    category_id integer,
    description text,
    is_active boolean DEFAULT true,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    modified_by integer,
    CONSTRAINT fk_productcategory_category FOREIGN KEY (category_id) REFERENCES category(category_id),
    CONSTRAINT fk_productcategory_createdby FOREIGN KEY (created_by) REFERENCES users(user_id),
    CONSTRAINT fk_productcategory_modifiedby FOREIGN KEY (modified_by) REFERENCES users(user_id)
);

-- Drop and create product table
DROP TABLE IF EXISTS product CASCADE;
CREATE TABLE product (
    product_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    product_name character varying(100),
    description text,
    is_active boolean DEFAULT true,
    mrp numeric,
    sales_price numeric,
    quantity integer,
    landing_price numeric DEFAULT 18.00,
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    modified_at timestamp without time zone,
    product_category_id integer,
    CONSTRAINT fk_product_category FOREIGN KEY (product_category_id) REFERENCES product_category(product_category_id),
    CONSTRAINT fk_created_by FOREIGN KEY (created_by) REFERENCES users(user_id)
);

-- Drop and create order_items table
DROP TABLE IF EXISTS order_items CASCADE;
CREATE TABLE order_items (
    order_item_id INT GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    order_id integer,
    product_id integer,
    quantity integer,
    unit_price numeric,
    sub_total numeric,
    discount_percent numeric DEFAULT 0,
    discount_amount numeric,
    serial_no character varying(15),
    created_at timestamp without time zone DEFAULT CURRENT_TIMESTAMP,
    created_by integer,
    CONSTRAINT fk_order_items_orders FOREIGN KEY (order_id) REFERENCES orders(order_id) ON DELETE CASCADE,
    CONSTRAINT fk_order_items_product FOREIGN KEY (product_id) REFERENCES product(product_id) ON DELETE RESTRICT,
    CONSTRAINT fk_order_items_users FOREIGN KEY (created_by) REFERENCES users(user_id) ON DELETE RESTRICT
);
