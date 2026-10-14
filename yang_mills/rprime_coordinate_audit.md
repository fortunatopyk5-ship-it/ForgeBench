# R-prime compatibility and coordinate audit — Iteration 9 (final)

Scope: ONLY CMP122-II (1.90) incompatibility and identification of the canonical exact-union logarithm with Balaban native R'^(k)(X). No upstream (1.91) analysis, no boundary B' estimates, no K_0, no STEP_0.

## 1. Source cross-version resolution

CMP122-II uses legacy references such as

(7.1) [I]

for the Mayer step and

(7.13) [I]

for the localized connected logarithm.

Its bibliography identifies [I] as CMP109 (1987). The published CMP109 version has only Sections 0--5 and explicitly says at its end that the localization/cluster expansion will be constructed in the next paper.

That next paper is CMP116 (1988), "Renormalization group approach ... II. Cluster expansions". Its Sect. 2 starts:

- the first step is the Mayer expansion, Eq. (2.1);
- the resulting polymer gas is Eq. (2.11);
- exponentiation is Eq. (2.12);
- the localized connected coefficient grouped by exact target union is Eq. (2.13).

CMP122-II also says the first operation near (1.90) is "the Mayer expansion ... the same as in (7.1) [I]".

Therefore the legacy manuscript numbering used by CMP122-II is source-crosswalked as

legacy (7.1) [I]  <->  published CMP116 (2.1),

legacy (7.13) [I] <-> published CMP116 (2.13).

This is not a guess from equality of total logarithms; it is a two-anchor cross-version reconstruction of the exact cluster-expansion passage.

Status:
SOURCE-CROSSWALK VERIFIED.

## 2. Exact prime / compatibility definition

CMP122-II pp.387--388 constructs a special decomposition

X'_0 = union_{p=1}^r X'_p

into connected localization components and resums at fixed components, giving (1.90)

1 + sum_{r>=1} sum'_{ {X'_1,...,X'_r} } product_p F(X'_p).

The exact compatibility convention of the corresponding published cluster construction is CMP116 Eq. (2.11).

For localization domains Z,Z', define

zeta(Z,Z')=0

iff Z cap Z' contains either

1. a pi_k cube, or
2. a full (d-1)-dimensional wall of a pi_k cube;

and zeta(Z,Z')=1 otherwise.

Thus the prime in (1.90), after the above legacy/published source crosswalk, is the hard-core restriction

product_{i<j} zeta(X'_i,X'_j)=1.

Equivalently, a family is compatible iff no two members share a cube and no cube of one member is wall-adjacent to a cube of another member.

This is stronger than literal core-cube overlap.

## 3. Exact incompatibility support

For a localization domain eta define its cube carrier

Car(eta)
 =
 {B in pi_k : B is one of the unit pi_k cubes whose union is eta}.

Let N_wall[S] denote the closed one-step neighborhood in the cube adjacency graph:

N_wall[S]
 =
 S union {B' : B' shares a full 3-wall with some B in S}.

Define

Supp_*(eta)=N_wall[Car(eta)].

Then in d=4,

eta not~ eta'
iff
Car(eta') cap Supp_*(eta) != empty.

This is exactly the cube-or-wall incompatibility of CMP116 (2.11).

No field collar or analytic-domain enlargement is being inserted here: Supp_* is only the polymer incompatibility support.

## 4. Correct anchor factor C_inc

Every four-dimensional cube has 2d=8 wall-neighbors.

Therefore

#Supp_*(eta)
<=
(2d+1)#Car(eta)
=
9 |eta|,

where |eta| is the number of pi_k cubes in the polymer.

Hence a rigorous uniform choice is

C_inc=9.

This need not be sharp because wall-neighborhoods of neighboring carrier cubes overlap, but it is exact as a sufficient volume/scale/cutoff-independent constant.

Consequently, if

q
 =
sup_B sum_{eta: B in Car(eta)}
 |F_eta| exp[a|eta|+g(eta)],

then for every eta_0,

sum_{eta not~ eta_0}
 |F_eta| exp[a|eta|+g(eta)]
<=
9 |eta_0| q.

Thus the sufficient KP condition is

9q<a.

## 5. Correct tree-stitch constant

CMP109 defines d_k(X) as the rescaled length of a shortest continuous-space tree contained in X and meeting all unit pi_k cubes of X.

For an incompatibility edge eta not~ eta':

- if the two polymers share a cube, the old cube-overlap stitching applies;
- in the worst native case, choose B in Car(eta) and B' in Car(eta') sharing a wall.

A shortest tree in eta meets B and a shortest tree in eta' meets B'. The union B union B' is a 4D rectangular box with side lengths

2,1,1,1,

so its Euclidean diameter is

sqrt(2^2+1^2+1^2+1^2)=sqrt(7).

Joining the two meeting points inside B union B' costs at most sqrt(7).

Therefore an incompatibility-connected family eta_1,...,eta_n with union gamma satisfies

d_k(gamma)
<=
sum_i d_k(eta_i)
+
sqrt(7)(n-1).

A rigorous native choice is

c_stitch=sqrt(7).

Again, optimality is not claimed.

## 6. Corrected KP radius

Use the generalized KP weights

a_KP(eta)=a|eta|,

g_theta(eta)
 =
theta d_k(eta)+sqrt(7) theta.

The previously proved d=4 coercivity is

|eta|<=16(d_k(eta)+1).

If

16a+theta<=mu_in,

then

a|eta|+g_theta(eta)
<=
mu_in d_k(eta)
+
16a+sqrt(7)theta.

Therefore on the anchored input ball ||F||_in<=rho,

q(rho)
 =
exp(16a+sqrt(7)theta) rho.

The native KP sufficient condition is

9 q(rho)<a.

Hence the corrected explicit radius is

r_*^native
 =
(a/9)
exp[-16a-sqrt(7)theta].

This REPLACES the Iteration-8 radius

a exp[-16a-2theta]

for the Balaban native polymer gas.

The old radius remains a valid theorem only for the artificial carrier-overlap-only canonical gas.

## 7. Canonical exact-union logarithm

For the native incompatibility function zeta above, define the Ursell coefficient

rho^T(eta_1,...,eta_n)
 =
sum_{G connected on {1,...,n}}
 product_{ij in E(G)}
 (zeta(eta_i,eta_j)-1).

Define

H_X^can(F)
 =
sum_{n>=1} 1/n!
sum_{eta_1,...,eta_n:
     union_i eta_i=X}
rho^T(eta_1,...,eta_n)
product_i F(eta_i).

This is precisely the mathematical form displayed in published CMP116 (2.12)--(2.13).

The exact-union rule immediately implies

D_eta H_X^can=0
unless
eta subset X.

Thus the derivative support collar remains

r_col=0.

The change from cube overlap to cube-or-wall incompatibility affects C_inc and c_stitch, but not this exact-union derivative support statement.

## 8. Balaban native R'^(k)(X)

CMP122-II (1.98) writes

exp R'^(k)
 =
exp sum_X R'^(k)(X).

The text immediately following states that each native R'^(k)(X) is given by the convergent series (7.13) [I], with the proper notational changes.

Using the source cross-version resolution in Sect. 1, this cited series is the published CMP116 exact-union formula (2.13).

Therefore, after the notational replacement

H(Z) -> F(X'),

E^(k+1)(X) -> R'^(k)(X),

the definition is coordinatewise

R'^(k)_Bal(X)
=
H_X^can(F).

No additional localization/extraction map L occurs between the connected Ursell sum and R'^(k)(X) in this source route.

Coordinate identification verdict:

EXACT.

Provenance label:
SOURCE-CROSSWALK VERIFIED, because the printed CMP122-II reference uses legacy numbering (7.13)[I], while the formula appears in the separately published continuation CMP116 as (2.13).

## 9. Revalidated native bounded C1 theorem

Let

mu_in=(1+beta)kappa-lambda_in>0

with C_geo(lambda_in)<infinity.

Choose a,theta>0 with

16a+theta<=mu_in,

and set

r_*^native
=
(a/9)e^{-16a-sqrt(7)theta}.

Choose gamma_R'>0 so small that

r
=
C_geo(lambda_in)e^{-p_0(gamma_R')}
<
r_*^native,

and

rho=(r+r_*^native)/2.

Define

q_native(rho)
=
e^{16a+sqrt(7)theta}rho.

Then

9q_native(rho)<a,

so the native cluster series is normally convergent in the anchored output space and gives a Banach-holomorphic map.

Because native coordinates equal the exact-union coefficients,

R'_Bal=H^can

coordinatewise.

Hence

sup_{||F||_in<=r}
||D R'_Bal(F)||
<=
q_native(rho)/(rho-r).

The constant is independent of finite volume, UV cutoff and RG depth.

Status:

BALABAN REGULAR R' COEFFICIENT MAP:
UNIFORM BOUNDED C1 PROVED

under the same explicit input decay-budget condition as before, but with the corrected native KP radius.

## 10. Quasi-local Schur

The component Cauchy bound becomes

||D_eta R'_Bal,X(F)||
<=
[q_native(rho)/(rho-r)]
e^{mu_in d_k(eta)}
e^{-theta d_k(X)}.

Exact union still gives eta subset X, so choose a deterministic selector

s(X,eta) in eta subset X.

Therefore r_col=0.

If

lambda_res
 =
theta-mu_out-nu

satisfies

C_geo(lambda_res)<infinity,

then

sup_{eta contains a}
[
 e^{-mu_in d_k(eta)}
 sum_{X contains b:s(X,eta)=a}
 e^{mu_out d_k(X)}
 ||D_eta R'_Bal,X(F)||
]
<=
[q_native(rho)/(rho-r)]
C_geo(lambda_res)
e^{-nu dist_k(a,b)}.

Thus the quasi-local weighted Schur theorem survives, with the corrected q_native and r_*.

Status:
PROVED CONDITIONAL ON THE SAME EXPLICIT ADDITIONAL DECAY-BUDGET
C_geo(theta-mu_out-nu)<infinity.

No numerical Balaban kappa threshold is claimed.

## Final Iteration-9 verdict

EXACT PRIME/COMPATIBILITY:
cube-or-wall hard core.

Supp_*:
closed one-step wall neighborhood of the cube carrier.

C_inc:
9.

c_stitch:
sqrt(7).

r_*:
(a/9) exp[-16a-sqrt(7)theta].

Coordinate identification:
EXACT after the legacy-to-published source crosswalk (7.13)[I] <-> CMP116 (2.13).

Native regular R' bounded C1:
PROVED with corrected constants.

Quasi-local Schur:
PROVED under the explicit residual summability reserve.

No upstream (1.91).
No boundary B' analysis.
No K_0.
No STEP_0.
