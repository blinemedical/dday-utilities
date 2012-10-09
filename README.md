dday-utilities
===================

[DDay](http://sourceforge.net/scm/?type=svn&group_id=187422) is an awesome iCalendar library, but, it lacks support for serializing large calendars. 
I came across a problem where I had to generate large calendars dynalically so I wrote the DDayCalendarWriter which is based on TextWriter so can support arbitrarily large 
calendars. You can read more about it at our blog here: http://tech.blinemedical.com/serializing-large-number-of-objects/