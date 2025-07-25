CREATE TABLE user_menu_permissions (
    user_menu_permission_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    user_id INTEGER NOT NULL REFERENCES users(user_id) ON DELETE CASCADE,
    menu_item_id INTEGER NOT NULL REFERENCES menu_items(menu_items_id) ON DELETE CASCADE,
    order_by INTEGER NOT NULL DEFAULT 0,
    CONSTRAINT uq_user_menu UNIQUE (user_id, menu_item_id)
)
 