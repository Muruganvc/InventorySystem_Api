ALTER TABLE public.order_items
DROP COLUMN IF EXISTS sub_total,
ADD COLUMN sub_total numeric GENERATED ALWAYS AS (quantity * unit_price) STORED;

-- Drop and recreate `discount_amount`
ALTER TABLE public.order_items
DROP COLUMN IF EXISTS discount_amount,
ADD COLUMN discount_amount numeric GENERATED ALWAYS AS (quantity * unit_price * (discount_percent / 100.0)) STORED;

-- Drop and recreate `net_total`
ALTER TABLE public.order_items
DROP COLUMN IF EXISTS net_total,
ADD COLUMN net_total numeric GENERATED ALWAYS AS (quantity * unit_price * (1 - discount_percent / 100.0)) STORED;


