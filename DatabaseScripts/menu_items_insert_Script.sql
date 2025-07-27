SELECT 
    'INSERT INTO public.menu_items (
         label, icon, route, parent_id, order_by) 
     VALUES (' ||
    quote_nullable(label) || ', ' ||
    quote_nullable(icon) || ', ' ||
    quote_nullable(route) || ', ' ||
    quote_nullable(parent_id) || ', ' ||
    quote_nullable(order_by) || 
    ');'
FROM public.menu_items
ORDER BY menu_items_id;
