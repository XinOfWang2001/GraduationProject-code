from datetime import datetime, timedelta


def get_interval(nanoseconds: float) -> timedelta:
    microseconds = nanoseconds / 10
    return timedelta(microseconds=microseconds)

def microseconds_to_datetime(microseconds: timedelta)-> datetime:
    return datetime(1970, 1, 1) + microseconds