# YM-RPRIME-CANONICAL-C1-1 — canonical exact-union cluster/log C^1 theorem (Iteration 9 audited)

Scope:
Audit-corrected theorem for the CANONICAL exact-union logarithm built from the CMP122-II (1.90) polymer activities. It is NOT currently identified coordinatewise with Balaban's native R'^(k)(X). No full history map, no K_0, no STEP_0, no physical source differentiation.

## Iteration 9 audit correction

Two Iteration-8 source identifications were too strong.

1. CMP122-II p.388 proves that the X'_p occurring in each actual primed family are components of a special decomposition of X'_0 and the expression factorizes in those components. Thus their pi_k-cube carriers are pairwise disjoint. The accessible primary passage does NOT state the converse that every pairwise carrier-disjoint family is admissible under the prime. Hence native incompatibility is not proved to be iff carrier overlap.

2. CMP122-II p.390 states that native R'^(k)(X) is given by the convergent series (7.13) [I]. The formula of that cited series has not been recovered in the accessible published CMP109 version. Equality of total logarithms does not imply equality of localization coordinates. Hence
R'_Bal(X)=H_X^can(F)
is NOT established.

All statements below using literal overlap, C_inc=1, c_stitch=2, exact-union support, and r_*=a exp[-16a-2theta] are therefore statements about the explicitly defined CANONICAL carrier-overlap cluster chart unless explicitly marked conditional/native.
See rprime_coordinate_audit.md.

## 1. Source object and regular projection

Fix one k-th R-operation and the source domains Y_1,...,Y_m that are held fixed in the exponentiation step.

CMP122-II Eq. (1.90) gives a polymer expansion

Xi(F)
 =
1+
sum_{r>=1}
sum'_{ {X'_1,...,X'_r} }
product_{p=1}^r F(X'_p),

where X'_p are connected localization components. Direct primary source verifies that members of each actual primed family arise as distinct components of the special decomposition and therefore have pairwise disjoint pi_k-cube carriers. The exact converse characterization of the prime is NOT source-extracted.

Eq. (1.91) defines each pre-exponentiation activity F(X') by resumming all raw expressions that determine exactly the connected localization domain X'.

Let

Ycal = union_{i=1}^m Y_i.

For a source component eta=X' disjoint from Ycal, primary Eq. (1.97) gives

||F_eta^src||
 <=
 exp[-p_0(g_k)]
 exp[-kappa_in d_k(eta)],                                 (R1)

where

kappa_in=(1+beta)kappa.

Reason:
Eq. (1.97) gives the relative metric d_{k,Ycal}; primary Eq. (1.67) defines it by a shortest tree intersecting components of eta\Ycal. If eta cap Ycal is empty, eta\Ycal=eta and therefore

d_{k,Ycal}(eta)=d_k(eta).

The source later splits the logarithmic output R'^(k)(X) into:
- first group: boundary B' terms;
- second group: regular R terms.

For the second group primary CMP122-II states that c_1=exp[-p_0(g_k)] and the ordinary d_k(X) replaces the relative metric, yielding Eq. (1.100).

We call this second group the regular R' sector.

## 2. Common coefficient algebra for coefficient-C^1

No physical field/source derivative is taken in this theorem.

For each finite-volume/source-history step, let Xscr_k be the set of admissible field/source configurations on which the local source activities in (1.90) are evaluated.

Let

A_k = B_b(Xscr_k; C)

with the sup norm and pointwise multiplication.

Every source local cylinder activity F_eta is pulled back to Xscr_k by restriction of the global configuration to eta. Thus all activities may be regarded as elements of the same commutative unital Banach algebra A_k, and

||fg||_{A_k}<=||f||_{A_k}||g||_{A_k}.

This is only a coefficient algebra. It makes no claim about a uniform complex collar in the physical U/J variables.

## 3. Exact regular input anchored norm

Let P_k^reg be the connected localization domains eta relevant to the regular lane, in particular eta cap Ycal=empty when using (R1).

For mu_in>=0 define

||F||_in
 :=
 sup_{B in pi_k}
 sum_{eta in P_k^reg: eta contains B}
 e^{mu_in d_k(eta)} ||F_eta||_{A_k}.                      (R2)

This defines the input Banach space B_in,k(mu_in).

Choose a fixed lambda_in in the CMP109 geometric-summability regime and set

mu_in
 :=
 kappa_in-lambda_in
 =
 (1+beta)kappa-lambda_in
 >0.                                                       (R3)

By CMP109 Eq. (0.26),

C_geo(lambda_in)
 :=
 sup_{k,B}
 sum_{eta contains B}
 e^{-lambda_in d_k(eta)}
 <infinity.                                                (R4)

Combining (R1)-(R4),

||F^src||_in
 <=
 r_src(g_k)
 :=
 C_geo(lambda_in)e^{-p_0(g_k)}.                           (R5)

This is the requested source pointwise -> anchored norm implication.

## 4. Primary small-coupling input

CMP119 introduction directly defines the large-field profile in the form

p_0(g)
 =
 A_0 (log g^{-2})^{p_*},                                  (R6)

with positive integer p_* (and the positive large-field constant A_0 in the construction).

Hence, on 0<g<=gamma<1,

p_0(g)>=p_0(gamma),

and

p_0(gamma)->infinity
as gamma->0.                                               (R7)

Therefore

r_src(g)
<=
r_src(gamma)
 :=
C_geo(lambda_in)e^{-p_0(gamma)}
 ->0                                                       (R8)

as gamma->0.

This is the only small-coupling fact needed below to place the actual source activity strictly inside an explicit activity ball.

The detailed CMP122-II derivation of (1.97) itself uses its own source restrictions; these are logically PRIOR to our activity-ball argument and are not replaced by it.

Direct-primary conditions visible in the derivation around (1.92)-(1.97) include sufficient inequalities of the source shapes

alpha_LF^(1/3)
<=
exp[-(1+beta)kappa*2d],

O(1) alpha_LF^(1/3)
<=
(6*2^d)^(-1) beta kappa,

and, after taking g_k sufficiently small,

exp[-p_0(g_k)/2]
<=
(6*2^d)^(-1) beta kappa.

The source also imposes its other stated large-K / localization restrictions needed for (1.97).

These conditions establish the concrete RG-generated F and its pointwise decay. They are NOT the radius of the coefficient-space cluster map.

Our separate generalized-KP condition in Section 6 establishes holomorphy for every activity family in an explicit anchored Banach ball.

## 5. Tree stitching for connected hard-core clusters

For a connected localization domain X, d_k(X) is the shortest-tree length in the rescaled pi_k-cube geometry.

For the CANONICAL carrier-overlap model, let eta_1,...,eta_n be polymers whose carrier-overlap graph is connected and let

gamma=union_i eta_i.

In d=4,

d_k(gamma)
 <=
 sum_i d_k(eta_i)
 + c_box(n-1),                                             (R9)

where one may take

c_box=sqrt(4)=2.

Proof:
Choose a shortest tree inside each eta_i. Choose a spanning tree of the overlap graph. For each spanning-tree edge, eta_i and eta_j share a unit pi_k cube. Their two internal trees each meet that cube. Join the two meeting points by a segment inside the common unit cube; this costs at most its Euclidean diameter 2. The union is a connected tree in gamma meeting every cube.

This is the geometric ingredient for the CANONICAL carrier-overlap chart. For native CMP122-II compatibility, c_stitch remains UNKNOWN until the prime is characterized exactly.

## 6. Explicit tree-weighted KP ball

Fix constants

a>0,
theta>0

such that

16a+theta <= mu_in.                                       (R10)

The factor 16 is the already proved d=4 coercivity

|eta|<=16(d_k(eta)+1).

Define the generalized KP weights

a_KP(eta)=a|eta|,

g_theta(eta)=theta d_k(eta)+2theta.                        (R11)

For any F in B_in,k(mu_in),

sup_B
sum_{eta contains B}
||F_eta||
exp[a_KP(eta)+g_theta(eta)]

<=
exp(16a+2theta)||F||_in.                                  (R12)

Indeed,
a|eta|+theta d(eta)+2theta
<=
(16a+theta)d(eta)+16a+2theta
<=
mu_in d(eta)+16a+2theta.

Set

q(rho)
 :=
 e^{16a+2theta} rho,                                      (R13)

and define the EXPLICIT sufficient convergence radius

r_*
 :=
 a e^{-16a-2theta}.                                       (R14)

If

||F||_in <= rho < r_*,

then q(rho)<a.

For every polymer eta_0 in the explicitly defined CANONICAL carrier-overlap system, incompatibility is carrier overlap, hence

sum_{eta incompatible with eta_0}
||F_eta|| e^{a_KP(eta)+g_theta(eta)}

<=
|eta_0| q(rho)
<
a|eta_0|
=
a_KP(eta_0).                                              (R15)

Thus the generalized Kotecky-Preiss criterion holds uniformly in finite volume, cutoff, and scale for the CANONICAL carrier-overlap system.

For a general native incompatibility certificate with
#Supp_*(eta)<=C_inc |eta|
and connection cost c_stitch per incompatibility-tree edge, the corrected formulas are

g_theta(eta)=theta d(eta)+c_stitch theta,

q(rho)=exp(16a+c_stitch theta) rho,

r_*^corr=(a/C_inc) exp[-16a-c_stitch theta].

For the canonical carrier-overlap chart C_inc=1 and c_stitch=2, recovering (R14).
Native C_inc and c_stitch are currently UNKNOWN.

No hidden "sufficiently small" activity radius is used in the canonical theorem.

## 7. Canonical cluster logarithm and exact-union support

Define the canonical connected logarithm of the source polymer gas (1.90) by

H_R'(F)_gamma
 :=
 sum_{n>=1} 1/n!
 sum_{eta_1,...,eta_n:
      incompatibility graph connected,
      union_i eta_i=gamma}
 phi^T(eta_1,...,eta_n)
 product_i F_{eta_i}.                                     (R16)

Here phi^T is the standard Ursell coefficient of the hard-core overlap graph.

Eq. (1.98) identifies the source expression as an exponentiated convergent connected cluster logarithm. We use the canonical exact-union localization of that logarithm:

Xi(F)=exp[sum_gamma H_R'(F)_gamma].

The exact-union grouping in (R16) is our canonical connected-log localization of the source polymer gas (1.90). It reconstructs the same cluster logarithm/exponentiation (1.98). The support rule below is therefore a theorem of this canonical source-faithful chart; it is not quoted as a verbatim support formula from Balaban's printed reference to series (7.13) [I].

### Exact support theorem

Let

M_{gamma eta}(F)
 :=
D_eta H_R'(F)_gamma.

Then

M_{gamma eta}(F)=0
unless eta subset gamma.                                  (R17)

Proof:
Every monomial contributing to H_R'(F)_gamma has union exactly gamma. Differentiation with respect to F_eta can be nonzero only if eta is one of its constituent polymers, hence eta subset gamma.

Fix once and for all a total order on pi_k cubes and define

s(gamma,eta)
 :=
 the least cube of eta.                                   (R18)

For every nonzero Jacobian entry,
s(gamma,eta) belongs to eta subset gamma.

Therefore the source/cluster collar for this coefficient map is

r_col=0.                                                   (R19)

### Regular-sector decoupling

If an output gamma belongs to the source second/regular group and is disjoint from the source boundary/large-field set, every constituent eta in (R16) is contained in gamma and is therefore also disjoint from that set. Hence boundary-lane input activities cannot contribute to a regular output coefficient.

Thus the CANONICAL regular coefficient is the regular projection of the canonical logarithm of the regular input activity family.

This does NOT establish coordinatewise equality with Balaban's native R'^(k)(X). That identification is OPEN because the source defines the native term through a cited series (7.13) [I] whose localization formula has not yet been recovered.

## 8. Uniform output bound and holomorphy

Let

B_out,k(theta)

be the anchored output activity space with norm

||K||_theta
 =
sup_B sum_{gamma contains B}
e^{theta d_k(gamma)}||K_gamma||_{A_k}.                    (R20)

The generalized KP rooted-cluster estimate together with the tree stitching (R9) yields

||H_R'(F)||_theta
<=
q(rho)                                                     (R21)

for every ||F||_in<=rho<r_*.

Sketch of the anchored estimate:
for an overlap-connected cluster with union gamma, (R9) gives

e^{theta d(gamma)}
<=
product_i e^{g_theta(eta_i)}.

If gamma contains B, mark one constituent eta_1 containing B. The 1/n! cluster coefficient and the sum over the possible mark are bounded by the standard rooted KP estimate. Summing the marked root gives exactly the left side of (R12), hence at most q(rho).

The cluster series is absolutely normally convergent in B_out,k(theta) on every closed input ball of radius rho<r_*. This statement is made directly on the countable anchored polymer Banach space; finite tori are restrictions of the same majorant.

Partial sums by cluster size/support are Banach-valued holomorphic polynomials in F. Normal convergence in the B_out norm therefore implies, by the Banach-valued Weierstrass theorem,

H_R':
B_{r_*}(B_in,k(mu_in))
 ->
B_out,k(theta)                                             (R22)

is holomorphic.

The estimate and holomorphy are uniform in:
- total volume;
- UV cutoff;
- RG depth k.

The spaces are scale-indexed, but the constants and norm structure are uniform.

## 9. Strict source radius

Choose the Balaban small-coupling threshold gamma_R'>0 so small that

r
 :=
 C_geo(lambda_in)e^{-p_0(gamma_R')}
 <
 r_*.                                                      (R23)

Existence follows from (R7)-(R8).

This is compatible with CMP122-II Theorem 1, whose hypothesis already permits the effective-coupling interval ]0,gamma] with gamma sufficiently small; we simply shrink that fixed threshold further if necessary.

For every source scale satisfying

0<g_k<=gamma_R',

the source regular input obeys

||F^src||_in<=r.                                          (R24)

Define

rho
 :=
 (r+r_*)/2,                                               (R25)

delta_hist
 :=
 rho-r
 =
 (r_*-r)/2
 >0.                                                       (R26)

Thus

r<rho<r_*

with all three radii independent of volume, cutoff, and RG depth.

No numerical value is invented.

## 10. Cauchy component derivative

Fix F with ||F||_in<=r.

For a unit vector h in the eta-coordinate algebra A_k, the coordinate injection satisfies

||I_eta h||_in
=
e^{mu_in d_k(eta)}.

Therefore

F+z I_eta h

stays in the rho-ball whenever

|z|
<
(rho-r)e^{-mu_in d_k(eta)}.

By Banach-valued Cauchy applied to the gamma-coordinate and (R21),

||M_{gamma eta}(F)||
<=
[q(rho)/(rho-r)]
e^{mu_in d_k(eta)}
e^{-theta d_k(gamma)}.                                   (R27)

This is a full-Banach-space Cauchy estimate, not a coordinatewise analyticity assumption.

## 11. Bounded C^1 closes before the Schur step

The bounded C^1 theorem does NOT require a second CMP109 summability reserve.

By Section 8, for every rho<r_*,

H_R':
B_rho(B_in,k(mu_in))
->
B_out,k(theta)

is holomorphic and

sup_{||F||_in<=rho}
||H_R'(F)||_theta
<=q(rho).

Hence Banach-valued Cauchy directly gives, on the smaller radius-r ball,

boxed:
sup_{||F||_in<=r}
||D H_R'(F)||_{B_in,k(mu_in)->B_out,k(theta)}
<=
q(rho)/(rho-r).                                           (R30)

This constant is independent of volume, cutoff and RG depth.

Thus source-uniform bounded C^1 of the regular R' coefficient map is already closed once the explicit strict radius in Section 9 has been obtained.

No numerical lower bound on the source localization exponent kappa is needed beyond the explicit input-budget condition

mu_in=(1+beta)kappa-lambda_in>0

for some lambda_in in the already verified CMP109 summability regime.

This condition is kept symbolic; no qualitative "kappa sufficiently large" statement is converted into a fake number.

## 12. Quasi-local Jacobian: additional decay-budget condition

For the stronger off-diagonal Jacobian estimate, use the component Cauchy bound

||M_{gamma eta}(F)||
<=
[q(rho)/(rho-r)]
e^{mu_in d_k(eta)}
e^{-theta d_k(gamma)}.                                   (R31)

The exact-union support theorem gives

M_{gamma eta}=0 unless eta subset gamma.

Let a=s(gamma,eta) be the fixed least cube of eta and let b be an output anchor in gamma. Then

d_k(gamma)>=dist_k(a,b).                                  (R32)

Choose nu>0 and mu_out>=0 such that

lambda_res
 :=
 theta-mu_out-nu                                          (R33)

lies in the CMP109 geometric-summability regime.

Under THIS ADDITIONAL EXPLICIT DECAY-BUDGET CONDITION,

sup_{eta contains a}
[
 e^{-mu_in d_k(eta)}
 sum_{gamma contains b:s(gamma,eta)=a}
 e^{mu_out d_k(gamma)}
 ||M_{gamma eta}(F)||
]

<=
[q(rho)/(rho-r)]
C_geo(lambda_res)
e^{-nu dist_k(a,b)}.                                      (R34)

Define

C_lat(nu)
 :=
sup_b sum_a e^{-nu dist_k(a,b)}<infinity.                 (R35)

Then weighted Schur gives

||D H_R'(F)||_{B_in,k(mu_in)->B_out,k(mu_out)}
<=
C_R'^Schur,                                                (R36)

where

C_R'^Schur
 :=
[q(rho)/(rho-r)]
C_geo(lambda_res)
C_lat(nu).                                                 (R37)

All constants are cutoff/volume/depth independent.

Important honesty point:
the existence of lambda_res in the CMP109 summability regime is NOT needed for bounded C^1 into B_out(theta). It is needed only for the stronger quasi-local Schur kernel statement. This run does not invent a numerical source threshold ensuring (R33).

## 13. YM-RPRIME-CANONICAL-C1-1

Theorem — bounded C^1 part:
Fix Balaban source parameters beta,kappa and choose:

1. lambda_in in the CMP109 summability regime with
   mu_in=(1+beta)kappa-lambda_in>0;

2. a,theta>0 satisfying
   16a+theta<=mu_in;

3. r_*=a exp[-16a-2theta];

4. gamma_R'>0 small enough that
   r=C_geo(lambda_in)e^{-p_0(gamma_R')}<r_*;

5. rho=(r+r_*)/2.

Then for every RG scale k with 0<g_k<=gamma_R', the CANONICAL carrier-overlap exact-union cluster/log map built from the regular CMP122-II (1.90) input activities,

H_R',k^reg:
B_r(B_in,k(mu_in))
->
B_out,k(theta),

is the restriction of a holomorphic map on the r_*-ball and therefore is C^1, with the uniform bound

boxed:
sup_{||F||_in<=r}
||D H_R',k^reg(F)||
<=
C_R'^C1
:=
q(rho)/(rho-r).                                           (R38)

The constant is independent of total volume, UV cutoff and RG depth.

The actual Balaban-generated regular activity satisfies the input-ball condition because

||F^src||_in
<=
C_geo(lambda_in)e^{-p_0(g_k)}
<=r.

Status of bounded C^1 theorem:
PROVED FOR THE CANONICAL CARRIER-OVERLAP EXACT-UNION CLUSTER/LOG CHART, under the explicit source decay-budget condition in item 1.

NATIVE BALABAN R' COORDINATE STATUS:
NOT PROVED / COORDINATE IDENTIFICATION OPEN.

No unnamed source convergence radius is used.

Stronger quasi-local part:
If, in addition, there exist mu_out,nu with (R33) in the CMP109 summability regime, then the Jacobian satisfies (R34) and the Schur operator bound (R36).

Status of quasi-local Jacobian:
PROVED UNDER THE EXPLICIT ADDITIONAL DECAY-BUDGET CONDITION (R33). A numerical Balaban parameter threshold for this extra reserve is not extracted.

Non-claims:
This theorem does not cover:
- the first/boundary B' group;
- the upstream construction of F from T/E/R/B data;
- the full compressed history map;
- physical source differentiation.

## 14. Decay-reserve wording

The source exponents before and after the Balaban exponentiation are

(1+beta)kappa
and
(1+beta/2)kappa.

Their difference

(beta/2)kappa

is recorded only as the

SOURCE-VISIBLE DECAY RESERVE.

The actual Banach operator loss in YM-RPRIME-CANONICAL-C1-1 is

mu_in-mu_out,

which also spends whatever geometric summability/off-diagonal reserves were chosen in lambda_in, theta, lambda_res, and nu.

These two quantities are not identified.

## 15. Source insertion map status

Introduce a physical source space J_A and an insertion map

jmath_A:J_A->B_in,k(mu_in).

No boundedness of jmath_A is proved here.

Therefore no statement of the form

J_A in B_in

is made automatically.

The present theorem is a coefficient-space C^1 result only.

## 16. Final Iteration-8 verdict

The canonical carrier-overlap exact-union cluster/log C^1 theorem is closed under an explicit source decay-budget inequality. The prior claim that this already proves bounded C^1 for Balaban's native R'^(k)(X) coordinates is withdrawn. Native closure now additionally requires exact prime/incompatibility identification and native-coordinate identification. The stronger canonical quasi-local Schur estimate is conditional on one additional explicit decay reserve.

The next blocker, outside this run's allowed scope, is not R' exponentiation. It is upstream:
the bounded C^1/source-local map that constructs the pre-exponentiation activity F from the earlier localized T/E/R/B history data (and, separately, the boundary B' lane).

No K_0.
No STEP_0.
