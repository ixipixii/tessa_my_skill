update st
set st.TaskTypeName = 
(
select Name
from Types t with(nolock)
where t.ID = st.TaskTypeID
)
from FdStageTemplate st
where st.TaskTypeID is not null