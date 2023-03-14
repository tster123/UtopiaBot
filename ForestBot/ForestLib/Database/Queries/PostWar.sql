-- ops total
select OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Damage'
from Operations
where Timestamp > '2023-03-07 17:00' and TargetKingdom = '1:12'
group by OpName
order by OpName

-- ops by source
select SourceProvince, OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Damage'
from Operations
where Timestamp > '2023-03-07 17:00' and TargetKingdom = '1:12'
group by OpName, SourceProvince
order by SourceProvince, OpName

-- ops by target
select TargetProvince, OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Damage'
from Operations
where Timestamp > '2023-03-07 17:00' and TargetKingdom = '1:12'
group by OpName, TargetProvince
order by TargetProvince, OpName

-- full ops breakdown
select SourceProvince, TargetProvince, OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Damage'
from Operations
where Timestamp > '2023-03-07 17:00' and TargetKingdom = '1:12'
group by OpName, TargetProvince, SourceProvince
order by SourceProvince, TargetProvince, OpName

-- support
select SourceProvince, OpName, count(*) as 'Attempts', sum(cast(Success as int)) as 'Success', sum(Damage) as 'Ticks'
from Operations
where Timestamp > '2023-03-07 17:00' and TargetKingdom is null and TargetProvince is not null
group by SourceProvince, OpName
order by SourceProvince, OpName

-- murder beast
select v.SourceProvince, sum(v.Damage)
from
(select SourceProvince, TargetProvince, Damage 
from Attacks
where TargetKingdom = '4:3' and AttackType = 'mass'
union all
select SourceProvince, TargetProvince, Kills + Prisoners 
from Attacks where TargetKingdom = '4:3'
union all
select SourceProvince, TargetProvince, Damage
from Operations 
where TargetKingdom = '4:3' and Success = 1 and OpName in ('fireball', 'nightstrike', 'assassinate wizards')
) v
group by v.SourceProvince
order by sum(v.Damage) DESC

-- murder methods
select v.SourceProvince, v.Method, sum(v.Damage)
from
(select SourceProvince, 'massacre' as Method, TargetProvince, Damage 
from Attacks
where TargetKingdom = '4:3' and AttackType = 'mass'
union all
select SourceProvince, 'casualty' as Method, TargetProvince, Kills + Prisoners 
from Attacks where TargetKingdom = '4:3'
union all
select SourceProvince, OpName as Method, TargetProvince, Damage
from Operations 
where TargetKingdom = '4:3' and Success = 1 and OpName in ('fireball', 'nightstrike', 'assassinate wizards')
) v
group by v.Method, v.SourceProvince  with rollup
order by SourceProvince desc

select top 1000 * from Operations where TargetKingdom is null order by Timestamp desc

select Timestamp, SourceProvince, TargetProvince, OpName, Success, Damage from Operations where TargetKingdom is null and SourceProvince = 'Not the Momma' and TargetProvince is not null

