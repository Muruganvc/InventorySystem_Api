CREATE TABLE menu_items (
    menu_items_id INTEGER GENERATED ALWAYS AS IDENTITY PRIMARY KEY,
    label VARCHAR(100) NOT NULL,
    icon VARCHAR(50),
    route VARCHAR(200),
    parent_id INTEGER,
    order_by INTEGER NOT NULL DEFAULT 0
);

-- Add a self-referencing foreign key constraint on parent_id
ALTER TABLE menu_items
ADD CONSTRAINT fk_menuitems_parent
FOREIGN KEY (parent_id) REFERENCES menu_items(menu_items_id) ON DELETE SET NULL;