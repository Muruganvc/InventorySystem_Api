-- Index on product_category_id in the product table
CREATE INDEX idx_product_product_category_id 
ON public.product(product_category_id);

-- Index on category_id in the product_category table
CREATE INDEX idx_product_category_category_id 
ON public.product_category(category_id);

-- Index on company_id in the category table
CREATE INDEX idx_category_company_id 
ON public.category(company_id);

-- Index on created_by in the product table
CREATE INDEX idx_product_created_by 
ON public.product(created_by);

-- Index on user_id in the users table
CREATE INDEX idx_users_user_id 
ON public.users(user_id);

-- Optionally: Composite Index on product_category_id and category_id in product table
-- This can speed up the query when filtering or joining on both columns
CREATE INDEX idx_product_product_category_and_category_id 
ON public.product(product_category_id, created_by);  -- Example composite index for your query's JOIN

-- Optionally: Composite Index on category_id and company_id for efficient joins on both columns
CREATE INDEX idx_category_category_id_and_company_id 
ON public.category(category_id, company_id);
