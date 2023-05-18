-- ops total
select OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Damage'
from Operations
where Timestamp > '2023-05-15 14:20' and TargetKingdom = '6:10'
group by OpName
order by OpName

-- ops by source
select SourceProvince, OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Damage'
from Operations
where Timestamp > '2023-05-15 14:20' and TargetKingdom = '6:10'
group by OpName, SourceProvince
order by SourceProvince, OpName

-- ops by target
select TargetProvince, OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Damage'
from Operations
where Timestamp > '2023-05-15 14:20' and TargetKingdom = '6:10'
group by OpName, TargetProvince
order by TargetProvince, OpName

-- full ops breakdown
select SourceProvince, TargetProvince, OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Damage'
from Operations
where Timestamp > '2023-05-15 14:20' and TargetKingdom = '6:10'
group by OpName, TargetProvince, SourceProvince
order by SourceProvince, TargetProvince, OpName

-- support
select SourceProvince, OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Ticks'
from Operations
where Timestamp > '2023-05-15 14:20' and TargetKingdom is null and TargetProvince is not null
group by SourceProvince, OpName
order by SourceProvince, OpName

-- murder beast
select v.SourceProvince, sum(v.Damage) as Murders
from
(select SourceProvince, TargetProvince, Damage 
from Attacks
where TargetKingdom = '6:10' and AttackType = 'mass' and Timestamp > '2023-05-15 14:20'
union all
select SourceProvince, TargetProvince, Kills + Prisoners 
from Attacks 
where TargetKingdom = '6:10' and Timestamp > '2023-05-15 14:20'
union all
select SourceProvince, TargetProvince, Damage
from Operations
where TargetKingdom = '6:10' and Success = 1 and OpName in ('fireball', 'night strike', 'assassinate wizards') and Timestamp > '2023-05-15 14:20'
) v
group by v.SourceProvince
order by sum(v.Damage) DESC

-- murder methods
select v.SourceProvince, v.Method, sum(v.Damage) as Murders
from
(select SourceProvince, 'massacre' as Method, TargetProvince, Damage 
from Attacks
where TargetKingdom = '6:10' and AttackType = 'mass' and Timestamp > '2023-05-15 14:20'
union all
select SourceProvince, 'casualty' as Method, TargetProvince, Kills + Prisoners 
from Attacks 
where TargetKingdom = '6:10' and Timestamp > '2023-05-15 14:20'
union all
select SourceProvince, OpName as Method, TargetProvince, Damage
from Operations 
where TargetKingdom = '6:10' and Success = 1 and OpName in ('fireball', 'night strike', 'assassinate wizards') and Timestamp > '2023-05-15 14:20'
) v
group by v.Method, v.SourceProvince  with rollup
order by SourceProvince desc

select top 1000 * from Operations where TargetKingdom is null order by Timestamp desc

select Timestamp, SourceProvince, TargetProvince, OpName, Success, Damage from Operations where TargetKingdom is null and SourceProvince = 'Not the Momma' and TargetProvince is not null

