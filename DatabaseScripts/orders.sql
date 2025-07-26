CREATE TABLE public.orders (
    order_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    customer_id INTEGER NOT NULL,
    order_date TIMESTAMPTZ DEFAULT CURRENT_TIMESTAMP,
    total_amount DECIMAL(18,2),
    final_amount DECIMAL(18,2),
    balance_amount DECIMAL(18,2),
    is_gst BOOLEAN NOT NULL DEFAULT FALSE,
    gst_number VARCHAR(15),

    CONSTRAINT fk_orders_customer
        FOREIGN KEY (customer_id)
        REFERENCES public.customers(customer_id)
        ON DELETE CASCADE
);
