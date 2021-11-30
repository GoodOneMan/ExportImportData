#############################################
## Comments
## pause loop 1 min //sec, min, hour [1:23:1]
## pause copy file 1 min //sec, min, hour
## file link D:\name.nwf
## file link D:\name.dgn
## file link D:\name.dwg
## file link D:\name.rvt
## file link D:\name.fbx
## group name DSSK_T200 > T200.nwd
## group file DSSK_T200 > D:\name.dwg
## folder save D:\...
## group folder DSSK_T200 > D:\...
#############################################
pause loop 1 min
pause copy file 1:29:13

group name 004_9_TM > 004_9_TM.nwd
group file 004_9_TM > D:\FileTest\004_9_TM\equip_2.dgn
group file 004_9_TM > D:\FileTest\004_9_TM\trubi_100.dgn
group file 004_9_TM > D:\FileTest\004_9_TM\trubi_200.dgn
group file 004_9_TM > D:\FileTest\004_9_TM\trubi_300.dgn
group file 004_9_TM > D:\FileTest\004_9_TM\trubi_400.dgn
group folder 004_9_TM > D:\ModelUpdate\004_9_TM\

folder save D:\ModelUpdate\OneFiles\
file link D:\FileTest\004_9_TM\trubi_200.dgn
file link D:\model_fbx\T400.fbx
file link D:\FileTest\T600_КР_7-88.rvt
file link D:\model_fbx\004_9_TM.dgn
file link D:\UpdateModelFiles\004_9_TM\trubi_100.nwf
file link D:\UpdateModelFiles\004_9_TM\trubi_200.nwf