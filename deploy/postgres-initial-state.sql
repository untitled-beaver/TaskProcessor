CREATE TYPE task_status AS ENUM ('created', 'running', 'finished');

CREATE TABLE public.tasks
(
    id uuid NOT NULL DEFAULT gen_random_uuid(),
    "time" timestamp without time zone NOT NULL,
    status task_status NOT NULL,
    PRIMARY KEY (id)
);
