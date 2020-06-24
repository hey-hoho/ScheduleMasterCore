CREATE INDEX scheduletraces_scheduleid_index ON scheduletraces (scheduleid);
CREATE INDEX scheduletraces_starttime_index ON scheduletraces (starttime DESC);
CREATE INDEX scheduletraces_result_index ON scheduletraces (result);
CREATE INDEX systemlogs_traceid_index ON systemlogs (traceid);
CREATE INDEX systemlogs_createtime_index ON systemlogs (createtime DESC);
CREATE INDEX scheduledelayeds_createtime_index ON scheduledelayeds (createtime DESC);
CREATE INDEX scheduledelayeds_contentkey_index ON scheduledelayeds (contentkey);