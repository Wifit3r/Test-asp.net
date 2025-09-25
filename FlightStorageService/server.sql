CREATE TABLE IF NOT EXISTS flights (
    flight_number VARCHAR(10) PRIMARY KEY,
    departure_date_time TIMESTAMP WITH TIME ZONE NOT NULL,
    departure_airport_city VARCHAR(100) NOT NULL,
    arrival_airport_city VARCHAR(100) NOT NULL,
    duration_minutes INTEGER NOT NULL
);

CREATE OR REPLACE FUNCTION add_flight(
    _flight_number VARCHAR(10),
    _departure_date_time TIMESTAMP WITH TIME ZONE,
    _departure_airport_city VARCHAR(100),
    _arrival_airport_city VARCHAR(100),
    _duration_minutes INTEGER
)
RETURNS VOID
LANGUAGE plpgsql
AS $$
BEGIN
    IF _departure_date_time BETWEEN NOW()::date AND (NOW()::date + INTERVAL '7 days') THEN
        INSERT INTO flights (
            flight_number,
            departure_date_time,
            departure_airport_city,
            arrival_airport_city,
            duration_minutes
        ) VALUES (
            _flight_number,
            _departure_date_time,
            _departure_airport_city,
            _arrival_airport_city,
            _duration_minutes
        );
    ELSE
        RAISE EXCEPTION 'Flight departure date must be within the next 7 days.';
    END IF;
END;
$$;

CREATE OR REPLACE FUNCTION get_flight_by_number(_flight_number VARCHAR(10))
RETURNS TABLE (
    flight_number VARCHAR(10),
    departure_date_time TIMESTAMP WITH TIME ZONE,
    departure_airport_city VARCHAR(100),
    arrival_airport_city VARCHAR(100),
    duration_minutes INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT f.* FROM flights AS f 
    WHERE f.flight_number = _flight_number;
END;
$$;

CREATE OR REPLACE FUNCTION get_flights_by_date(_date DATE)
RETURNS TABLE (
    flight_number VARCHAR(10),
    departure_date_time TIMESTAMP WITH TIME ZONE,
    departure_airport_city VARCHAR(100),
    arrival_airport_city VARCHAR(100),
    duration_minutes INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT f.* 
    FROM flights AS f 
    WHERE f.departure_date_time >= _date::timestamptz
      AND f.departure_date_time < (_date::timestamptz + INTERVAL '1 day');
END;
$$;

CREATE OR REPLACE FUNCTION get_flights_by_departure_city_and_date(
    _city VARCHAR(100),
    _date DATE
)
RETURNS TABLE (
    flight_number VARCHAR(10),
    departure_date_time TIMESTAMP WITH TIME ZONE,
    departure_airport_city VARCHAR(100),
    arrival_airport_city VARCHAR(100),
    duration_minutes INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT f.* 
    FROM flights AS f
    WHERE f.departure_airport_city ILIKE _city
      AND f.departure_date_time >= _date::timestamptz
      AND f.departure_date_time < (_date::timestamptz + INTERVAL '1 day');
END;
$$;

CREATE OR REPLACE FUNCTION get_flights_by_arrival_city_and_date(
    _city VARCHAR(100),
    _date DATE
)
RETURNS TABLE (
    flight_number VARCHAR(10),
    departure_date_time TIMESTAMP WITH TIME ZONE,
    departure_airport_city VARCHAR(100),
    arrival_airport_city VARCHAR(100),
    duration_minutes INTEGER
)
LANGUAGE plpgsql
AS $$
BEGIN
    RETURN QUERY
    SELECT f.* 
    FROM flights AS f
    WHERE f.arrival_airport_city ILIKE _city
      AND f.departure_date_time >= _date::timestamptz
      AND f.departure_date_time < (_date::timestamptz + INTERVAL '1 day');
END;
$$;