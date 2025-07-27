SELECT 
    'INSERT INTO public.users (
        first_name, last_name, user_name, password_hash, email, is_active, password_last_changed, 
        is_password_expired, last_login, mobile_no, profile_image, 
        created_by, created_date, modified_by, modified_date) 
     VALUES (' ||
    quote_nullable(first_name) || ', ' ||
    quote_nullable(last_name) || ', ' ||
    quote_nullable(user_name) || ', ' ||
    quote_nullable(password_hash) || ', ' ||
    quote_nullable(email) || ', ' ||
    quote_nullable(is_active) || ', ' ||
    quote_nullable(password_last_changed) || ', ' ||
    quote_nullable(is_password_expired) || ', ' ||
    quote_nullable(last_login) || ', ' ||
    quote_nullable(mobile_no) || ', ' ||
    quote_nullable(profile_image) || ', ' ||
    quote_nullable(created_by) || ', ' ||
    quote_nullable(created_date) || ', ' ||
    quote_nullable(modified_by) || ', ' ||
    quote_nullable(modified_date) || 
    ');'
FROM public.users
ORDER BY user_id;
