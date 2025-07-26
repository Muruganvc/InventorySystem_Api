CREATE TABLE public.audit_logs (
    audit_log_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    table_name VARCHAR(100),
    action VARCHAR(50),
    key_values TEXT,
    old_values TEXT,
    new_values TEXT,
    changed_by VARCHAR(100),
    changed_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
