CREATE TABLE public.order_items (
    order_item_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    order_id INTEGER NOT NULL,
    product_id INTEGER NOT NULL,
	serial_no VARCHAR(15),
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    unit_price NUMERIC(18,2) NOT NULL,
    discount_percent NUMERIC(5,2) DEFAULT 0,
    sub_total NUMERIC(18,2) GENERATED ALWAYS AS (quantity * unit_price) STORED,
    discount_amount NUMERIC(18,2) GENERATED ALWAYS AS ((quantity * unit_price * discount_percent) / 100.0) STORED,
    net_total NUMERIC(18,2) GENERATED ALWAYS AS ((quantity * unit_price) - ((quantity * unit_price * discount_percent) / 100.0)) STORED,
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP,
    created_by INTEGER NOT NULL,
    -- Foreign Key Constraints
    CONSTRAINT fk_order_items_orders FOREIGN KEY (order_id) REFERENCES public.orders(order_id) ON DELETE CASCADE,
    CONSTRAINT fk_order_items_product FOREIGN KEY (product_id) REFERENCES public.product(product_id) ON DELETE RESTRICT,
    CONSTRAINT fk_order_items_users FOREIGN KEY (created_by) REFERENCES public.users(user_id) ON DELETE RESTRICT
);
